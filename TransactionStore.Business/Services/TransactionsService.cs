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

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();
    private readonly IMapper _mapper;
    private readonly IValidator<DepositWithdrawRequest> _addDepositWithdrawValidator;
    private readonly IValidator<TransferRequest> _addTransferValidator;

    public TransactionsService(ITransactionsRepository transactionsRepository, IMapper mapper,
        IValidator<DepositWithdrawRequest> addDepositWithdrawValidator,
        IValidator<TransferRequest> addTransferValidator)
    {
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
        _addDepositWithdrawValidator = addDepositWithdrawValidator;
        _addTransferValidator = addTransferValidator;
    }

    public async Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = await _transactionsRepository.GetTransactionsByAccountIdAsync(id);
        var balance = transactions.Sum(t => t.Amount);

        _logger.Information("Counting and transmitting the balance. / Считаем и передаем баланс.");
        AccountBalanceResponse accountBalance = new AccountBalanceResponse()
        {
            AccountId = transactions[0].AccountId,
            Balance = balance,
            CurrencyType = transactions[0].CurrencyType
        };

        return accountBalance;
    }

    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = await _transactionsRepository.GetTransactionsByAccountIdAsync(id);
        return _mapper.Map<List<TransactionResponse>>(transactions);
    }

    public async Task<List<TransactionWithAccountIdResponse>> GetTransactionsByLeadIdAsync(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = await _transactionsRepository.GetTransactionsByLeadIdAsync(id);
        return _mapper.Map<List<TransactionWithAccountIdResponse>>(transactions);
    }

    public async Task<Guid> AddDepositWithdrawTransactionAsync(TransactionType transactionType, DepositWithdrawRequest request)
    {
        var validationResult = await _addDepositWithdrawValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            TransactionDto transaction = _mapper.Map<TransactionDto>(request);
            switch (transactionType)
            {
                case TransactionType.Deposit:
                    break;

                case TransactionType.Withdraw:
                    transaction.Amount *= -1;
                    break;

                default:
                    throw new Core.Exceptions.ValidationException("The transaction type must be deposit or withdrawal. / Тип транзакции должен быть deposit или withdraw.");
            }

            transaction.TransactionType = transactionType;

            return await _transactionsRepository.AddDepositWithdrawTransactionAsync(transaction);
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public async Task AddTransferTransactionAsync(TransferRequest request)
    {
        var validationResult = await _addTransferValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            var transferWithdraw = await CreateWithdrawTransactionAsync(request);
            var transferDeposit = await CreateDepositTransactionAsync(request);

            await _transactionsRepository.AddTransferTransactionAsync(transferWithdraw, transferDeposit);
        }
        else
        {
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }

    public async Task<TransactionDto> CreateWithdrawTransactionAsync(TransferRequest request)
    {
        return await Task.FromResult(new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1
        });
    }

    public async Task<TransactionDto> CreateDepositTransactionAsync(TransferRequest request)
    {
        var currencyRatesProvider = new CurrencyRatesProvider();
        var rateToUSD = await currencyRatesProvider.ConvertFirstCurrencyToUsdAsync(request.CurrencyFromType);
        var amountUsd = request.Amount * rateToUSD;
        var rateFromUsd = await currencyRatesProvider.ConvertUsdToSecondCurrencyAsync(request.CurrencyToType);

        return new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = amountUsd * rateFromUsd
        };
    }
}