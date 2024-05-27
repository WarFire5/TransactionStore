using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;
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
        _logger.Information("Вызываем метод репозитория");
        List<TransactionDto> transactionDtos = _transactionsRepository.GetTransactionsByAccountId(id);
        var balance = transactionDtos.Sum(t => t.Amount);

        _logger.Information("Считаем и передаем баланс");
        AccountBalanceResponse accountBalance = new AccountBalanceResponse()
        {
            AccountId = transactionDtos[0].AccountId,
            Balance = balance,
            CurrencyType = transactionDtos[0].CurrencyType
        };
       
        return accountBalance;
    }

    public Guid AddDepositWithdrawTransaction(TransactionType transactionType, DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

        if (validationResult.IsValid)
        {
            TransactionDto transaction;
            switch (transactionType)
            {
                case TransactionType.Deposit:
                    transaction = _mapper.Map<TransactionDto>(request);
                    break;

                case TransactionType.Withdraw:
                    transaction = _mapper.Map<TransactionDto>(request);
                    transaction.Amount *= -1;
                    break;

                default:
                    throw new Core.Exceptions.ValidationException("Тип транзакции должен быть deposit или withdraw.");
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

    private TransactionDto CreateWithdrawTransaction(TransferRequest request)
    {
        return new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1
        };
    }

    private TransactionDto CreateDepositTransaction(TransferRequest request)
    {
        var dictionaryOfCoefficients = new CurrencyRatesProvider();
        var rateToUSD = dictionaryOfCoefficients.GetRateToUsd(request.CurrencyFromType.ToString());
        var amountUsd = request.Amount * rateToUSD;
        var rateFromUsd = dictionaryOfCoefficients.GetRateFromUsd(request.CurrencyToType.ToString());

        return new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = amountUsd * rateFromUsd
        };
    }
}