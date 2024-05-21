using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Models.Transactions.Requests;
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

    public Guid AddDepositTransaction(DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

        if (validationResult.IsValid)
        {
            TransactionDto transaction = new TransactionDto()
            {
                AccountId = request.AccountId,
                TransactionType = request.TransactionType,
                CurrencyType = request.CurrencyType,
                Amount = request.Amount,
                Date = request.Date
            };

            return _transactionsRepository.AddDepositTransaction(transaction);
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public Guid AddWithdrawTransaction(DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

        if (validationResult.IsValid)
        {
            TransactionDto transaction = new TransactionDto()
            {
                AccountId = request.AccountId,
                TransactionType = request.TransactionType,
                CurrencyType = request.CurrencyType,
                Amount = request.Amount * -1,
                Date = request.Date
            };

            return _transactionsRepository.AddWithdrawTransaction(transaction);
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
                TransactionType = request.TransactionType,
                CurrencyType = request.CurrencyFromType,
                Amount = request.AmountFrom * -1,
                Date = request.Date
            };

            TransactionDto transferDeposit = new TransactionDto()
            {
                AccountId = request.AccountToId,
                TransactionType = request.TransactionType,
                CurrencyType = request.CurrencyToType,
                Amount = request.AmountTo,
                Date = request.Date
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