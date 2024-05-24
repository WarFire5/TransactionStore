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
        List<TransactionDto> transactionDtos = _transactionsRepository.GetBalanceByAccountId(id);
        var balance = transactionDtos.Sum(t => t.Amount);

        _logger.Information("Считаем и передаем баланс");
        TransactionDto accountBalance = new TransactionDto();
        accountBalance.Amount = balance;
        accountBalance.AccountId = transactionDtos[0].AccountId;
        accountBalance.CurrencyType = transactionDtos[0].CurrencyType;

        return _mapper.Map<AccountBalanceResponse>(accountBalance);
    }

    public Guid AddDepositWithdrawTransaction(DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

        if (validationResult.IsValid)
        {
            if (request.TransactionType == TransactionType.Deposit)
            {
                TransactionDto deposit = new TransactionDto()
                {
                    AccountId = request.AccountId,
                    TransactionType = request.TransactionType,
                    CurrencyType = request.CurrencyType,
                    Amount = request.Amount
                };

                return _transactionsRepository.AddDepositWithdrawTransaction(deposit);
            }

            TransactionDto withdraw = new TransactionDto()
            {
                AccountId = request.AccountId,
                TransactionType = request.TransactionType,
                CurrencyType = request.CurrencyType,
                Amount = request.Amount * -1
            };

            return _transactionsRepository.AddDepositWithdrawTransaction(withdraw);
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public void AddTransferTransaction(TransferRequest request)
    {
        var validationResult = _addTransferValidator.Validate(request);

        if (validationResult.IsValid)
        {
            TransactionDto transferWithdraw = new TransactionDto()
            {
                AccountId = request.AccountFromId,
                TransactionType = TransactionType.Transfer,
                CurrencyType = request.CurrencyFromType,
                Amount = request.Amount * -1
            };

            CurrencyRatesProvider dictionaryOfCoefficients = new CurrencyRatesProvider();
            var coef = dictionaryOfCoefficients.GetRate(request.CurrencyFromType.ToString(), request.CurrencyToType.ToString());

            TransactionDto transferDeposit = new TransactionDto()
            {
                AccountId = request.AccountToId,
                TransactionType = TransactionType.Transfer,
                CurrencyType = request.CurrencyToType,
                Amount = request.Amount * coef
            };

            _transactionsRepository.AddTransferTransaction(transferWithdraw, transferDeposit);
        }

        else
        {
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }
}