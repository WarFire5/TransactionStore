using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;
using TransactionStore.DataLayer.Repositories;
using ValidationException = FluentValidation.ValidationException;

namespace TransactionStore.Business.Services;

public class TransactionsService(ITransactionsRepository transactionsRepository, IMapper mapper,
    IValidator<DepositWithdrawRequest> addDepositWithdrawValidator,
    IValidator<TransferRequest> addTransferValidator) : ITransactionsService
{
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();

    public async Task<Guid> AddDepositWithdrawTransactionAsync(TransactionType transactionType, DepositWithdrawRequest request)
    {
        var validationResult = await addDepositWithdrawValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            TransactionDto transaction = mapper.Map<TransactionDto>(request);

            var commissionsProvider = new CommissionsProvider();
            var commissionPercent = commissionsProvider.GetPercentForTransaction(transactionType);
            var commissionAmount = transaction.Amount * commissionPercent / 100;

            switch (transactionType)
            {
                case TransactionType.Deposit:
                    transaction.Amount -= commissionAmount;
                    break;

                case TransactionType.Withdraw:
                    transaction.Amount = -(transaction.Amount + commissionAmount);
                    break;

                default:
                    throw new Core.Exceptions.ValidationException("The transaction type must be deposit or withdrawal. / Тип транзакции должен быть deposit или withdraw.");
            }

            transaction.TransactionType = transactionType;

            return await transactionsRepository.AddDepositWithdrawTransactionAsync(transaction);
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public async Task<TransferGuidsResponse> AddTransferTransactionAsync(TransferRequest request)
    {
        var validationResult = await addTransferValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            var commissionsProvider = new CommissionsProvider();
            var commissionPercent = commissionsProvider.GetPercentForTransaction(TransactionType.Transfer);
            var commissionAmount = request.Amount * commissionPercent / 100;

            var transferWithdraw = CreateWithdrawTransaction(request, commissionAmount);
            var transferDeposit = CreateDepositTransaction(request);

            var response = await transactionsRepository.AddTransferTransactionAsync(transferWithdraw, transferDeposit);
            return response;
        }
        else
        {
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }

    public static TransactionDto CreateWithdrawTransaction(TransferRequest request, decimal commissionAmount)
    {
        return new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1 - commissionAmount
        };
    }

    public static TransactionDto CreateDepositTransaction(TransferRequest request)
    {
        var currencyRatesProvider = new CurrencyRatesProvider();
        var rateToUSD = currencyRatesProvider.ConvertFirstCurrencyToUsd(request.CurrencyFromType);
        var amountUsd = request.Amount * rateToUSD;
        var rateFromUsd = currencyRatesProvider.ConvertUsdToSecondCurrency(request.CurrencyToType);

        return new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = amountUsd * rateFromUsd
        };
    }

    public async Task<List<TransactionWithAccountIdResponse>> GetTransactionByIdAsync(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionByIdAsync(id);
        return mapper.Map<List<TransactionWithAccountIdResponse>>(transactions);
    }

    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionsByAccountIdAsync(id);
        return mapper.Map<List<TransactionResponse>>(transactions);
    }

    public async Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = await transactionsRepository.GetTransactionsByAccountIdAsync(id);

        if (transactions.Count == 0)
        {
            _logger.Warning("No transactions found for the account. / Транзакций для переданного accountId не найдено.");
            return new AccountBalanceResponse
            {
                AccountId = id,
                Balance = 0,
                CurrencyType = CurrencyType.Unknown
            };
        }

        var balance = transactions.Sum(t => t.Amount);

        _logger.Information("Counting and transmitting the balance. / Считаем и передаем баланс.");
        AccountBalanceResponse accountBalance = new()
        {
            AccountId = transactions[0].AccountId,
            Balance = balance,
            CurrencyType = transactions[0].CurrencyType
        };

        return accountBalance;
    }
}