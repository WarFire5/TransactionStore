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

    public AccountBalanceResponse GetBalanceByAccountId(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = _transactionsRepository.GetTransactionsByAccountId(id);
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

    public List<TransactionResponse> GetTransactionsByAccountId(Guid id) 
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = _transactionsRepository.GetTransactionsByAccountId(id);
        return _mapper.Map<List<TransactionResponse>>(transactions);
    }

    public List<TransactionWithAccountIdResponse> GetTransactionsByLeadId(Guid id)
    {
        _logger.Information("Calling the repository method. / Вызываем метод репозитория.");
        List<TransactionDto> transactions = _transactionsRepository.GetTransactionsByLeadId(id);
        return _mapper.Map<List<TransactionWithAccountIdResponse>>(transactions);
    }

    public Guid AddDepositWithdrawTransaction(TransactionType transactionType, DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

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

            return _transactionsRepository.AddDepositWithdrawTransaction(transaction);
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public void AddTransferTransaction(TransferRequest request)
    {
        var validationResult = _addTransferValidator.Validate(request);

        if (validationResult.IsValid)
        {
            var transferWithdraw = CreateWithdrawTransaction(request);
            var transferDeposit = CreateDepositTransaction(request);

            _transactionsRepository.AddTransferTransaction(transferWithdraw, transferDeposit);
        }
        else
        {
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }

    public TransactionDto CreateWithdrawTransaction(TransferRequest request)
    {
        return new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1
        };
    }

    public TransactionDto CreateDepositTransaction(TransferRequest request)
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
}