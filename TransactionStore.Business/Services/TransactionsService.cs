using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.Business.Services;

public class TransactionsService(ITransactionsRepository transactionsRepository, ICurrencyRatesProvider currencyRatesProvider,
    ICommissionsProvider commissionsProvider, IMapper mapper, IMessagesService messagesService,
    IValidator<DepositWithdrawRequest> addDepositWithdrawValidator, IValidator<TransferRequest> addTransferValidator) : ITransactionsService
{
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();
    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);

    public async Task<Guid> AddDepositWithdrawTransactionAsync(TransactionType transactionType, DepositWithdrawRequest request)
    {
        _logger.Information("Validating the request asynchronously.");
        var validationResult = await addDepositWithdrawValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return await ProcessDepositWithdrawTransactionAsync(transactionType, request);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        else
        {
            _logger.Information("Handling validation errors.");
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
            throw new ValidationException(exceptions);
        }
    }

    private async Task<Guid> ProcessDepositWithdrawTransactionAsync(TransactionType transactionType, DepositWithdrawRequest request)
    {
        _logger.Information("Mapping the request object to TransactionDto.");
        TransactionDto transaction = mapper.Map<TransactionDto>(request);

        _logger.Information("A commission provider instance is created to obtain the commission percentage.");
        var commissionPercent = commissionsProvider.GetPercentForTransaction(transactionType);

        _logger.Information("Calculating the commission amount based on the transaction amount and commission percent.");
        var commissionAmount = transaction.Amount * commissionPercent / 100;

        switch (transactionType)
        {
            case TransactionType.Deposit:
                _logger.Information("Subtracting commission amount from deposit transaction amount.");
                transaction.Amount -= commissionAmount;
                break;

            case TransactionType.Withdraw:
                _logger.Information("Subtracting commission amount from withdraw transaction amount and making it negative.");
                transaction.Amount = -(transaction.Amount - commissionAmount);
                break;

            default:
                _logger.Information("Throwing an error if the transaction type is not suitable.");
                throw new Core.Exceptions.ValidationException("The transaction type must be deposit or withdrawal.");
        }

        _logger.Information("Setting the transaction type.");
        transaction.TransactionType = transactionType;

        var transactionId = await transactionsRepository.AddDepositWithdrawTransactionAsync(transaction);
        var transactionsDto = await transactionsRepository.GetTransactionByIdAsync(transactionId);

        _logger.Information("Convert transaction amount to RUB.");
        var rateToUSD = currencyRatesProvider.ConvertFirstCurrencyToUsd(request.Currency);
        var amountInUSD = transaction.Amount * rateToUSD;
        var rateToRUB = currencyRatesProvider.ConvertUsdToSecondCurrency(Currency.RUB);
        var amountInRUB = amountInUSD * rateToRUB;

        await messagesService.PublishTransactionAsync(transactionsDto, request.Currency, commissionAmount, amountInRUB);

        return transactionId;
    }

    public async Task<TransferGuidsResponse> AddTransferTransactionAsync(TransferRequest request)
    {
        _logger.Information("Validating the transfer request asynchronously.");
        var validationResult = await addTransferValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return await ProcessTransferTransactionAsync(request);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        else
        {
            _logger.Information("Throwing an error if validation fails.");
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }

    private async Task<TransferGuidsResponse> ProcessTransferTransactionAsync(TransferRequest request)
    {
        _logger.Information("Calculating the commission.");
        var commissionPercent = commissionsProvider.GetPercentForTransaction(TransactionType.Transfer);
        var commissionAmount = request.Amount * commissionPercent / 100;


        _logger.Information("Creating the withdraw transaction with commission.");
        var (transferWithdraw, withdrawAmount) = CreateWithdrawTransaction(request, commissionAmount);

        _logger.Information("Creating the deposit transaction.");
        var transferDeposit = CreateDepositTransaction(request, withdrawAmount);

        _logger.Information("Converting withdraw and deposit amounts to RUB.");
        var withdrawAmountInUsd = withdrawAmount * currencyRatesProvider.ConvertFirstCurrencyToUsd(request.CurrencyFrom);
        var withdrawAmountInRub = withdrawAmountInUsd * currencyRatesProvider.ConvertUsdToSecondCurrency(Currency.RUB);

        var depositAmountInUsd = transferDeposit.Amount * currencyRatesProvider.ConvertFirstCurrencyToUsd(request.CurrencyTo);
        var depositAmountInRub = depositAmountInUsd * currencyRatesProvider.ConvertUsdToSecondCurrency(Currency.RUB);

        _logger.Information("Adding the transfer transaction to the repository and getting the response.");
        var response = await transactionsRepository.AddTransferTransactionAsync(transferWithdraw, transferDeposit);

        _logger.Information("Creating list of transactions for message publishing.");
        var transactions = new List<TransactionDto> { transferWithdraw, transferDeposit };

        _logger.Information("Publishing transaction details to the message service.");
        await messagesService.PublishTransactionAsync(transactions, request.CurrencyFrom, commissionAmount, withdrawAmountInRub);
        await messagesService.PublishTransactionAsync(transactions, request.CurrencyTo, commissionAmount, depositAmountInRub);

        return response;
    }

    public (TransactionDto, decimal) CreateWithdrawTransaction(TransferRequest request, decimal commissionAmount)
    {
        _logger.Information("Creating withdraw transaction DTO.");
        var withdrawAmount = request.Amount - commissionAmount;
        return (new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            Amount = -withdrawAmount
        }, withdrawAmount);
    }

    public TransactionDto CreateDepositTransaction(TransferRequest request, decimal withdrawAmount)
    {
        _logger.Information("Getting currency conversion rates and calculating deposit amount.");
        var rateToUSD = currencyRatesProvider.ConvertFirstCurrencyToUsd(request.CurrencyFrom);
        var amountUsd = withdrawAmount * rateToUSD;
        var rateFromUsd = currencyRatesProvider.ConvertUsdToSecondCurrency(request.CurrencyTo);

        _logger.Information("Creating deposit transaction DTO.");
        return new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            Amount = amountUsd * rateFromUsd
        };
    }

    public async Task<FullTransactionResponse> GetTransactionByIdAsync(Guid id)
    {
        _logger.Information($"Getting transaction by ID {id}.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionByIdAsync(id);

        if (transactions == null || transactions.Count == 0)
        {
            _logger.Information($"Transactions for ID {id} not found.");
            return null;
        }

        var transactionResponse = new FullTransactionResponse();

        if (transactions[0].TransactionType == TransactionType.Deposit)
        {
            transactionResponse.Id = transactions[0].Id;
            transactionResponse.AccountToId = transactions[0].AccountId;
            transactionResponse.TransactionType = transactions[0].TransactionType;
            transactionResponse.Amount = transactions[0].Amount;
            transactionResponse.Date = transactions[0].Date;
        }
        else if (transactions[0].TransactionType == TransactionType.Withdraw)
        {
            transactionResponse.Id = transactions[0].Id;
            transactionResponse.AccountFromId = transactions[0].AccountId;
            transactionResponse.TransactionType = transactions[0].TransactionType;
            transactionResponse.Amount = transactions[0].Amount;
            transactionResponse.Date = transactions[0].Date;
        }
        else
        {
            transactionResponse.Id = transactions[1].Id;
            transactionResponse.AccountFromId = transactions[1].AccountId;
            transactionResponse.AccountToId = transactions[0].AccountId;
            transactionResponse.TransactionType = transactions[0].TransactionType;
            transactionResponse.Amount = transactions[1].Amount;
            transactionResponse.Date = transactions[0].Date;
        }

        return transactionResponse;
    }

    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
    {
        _logger.Information($"Getting transactions for account with ID {id}.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionsByAccountIdAsync(id);
        return mapper.Map<List<TransactionResponse>>(transactions);
    }

    public async Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            return await ProcessGetBalanceByAccountIdAsync(id);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task<AccountBalanceResponse> ProcessGetBalanceByAccountIdAsync(Guid id)
    {
        _logger.Information($"Getting a list of transactions for account with ID {id}.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionsByAccountIdAsync(id);

        if (transactions.Count == 0)
        {
            _logger.Information("No transactions found for the account.");
            return new AccountBalanceResponse
            {
                AccountId = id,
                Balance = 0,
            };
        }

        var balance = transactions.Sum(t => t.Amount);

        _logger.Information($"For an account with ID {id} the balance was calculated - {balance}.");
        AccountBalanceResponse accountBalance = new()
        {
            AccountId = transactions[0].AccountId,
            Balance = balance
        };

        return accountBalance;
    }
}