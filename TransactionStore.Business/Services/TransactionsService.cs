using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Exceptions;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;
using TransactionStore.DataLayer.Repositories;
using TransactionStore.Models.Exceptions;
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
                TransactionType = Core.Enums.TransactionType.Deposit,
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
            var balance = GetBalanceByAccountId(request.AccountId);

            if (request.Amount <= balance)
            {
                TransactionDto transaction = new TransactionDto()
                {
                    AccountId = request.AccountId,
                    TransactionType = Core.Enums.TransactionType.Withdraw,
                    CurrencyType = request.CurrencyType,
                    Amount = request.Amount * -1,
                    Date = request.Date
                };

                return _transactionsRepository.AddWithdrawTransaction(transaction);
            }

            throw new NotEnoughMoneyException("На балансе недостаточно средств для проведения операции.");
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public void AddTransferTransaction(TransferRequest request)
    {
        var validationResult = _addTransferValidator.Validate(request);

        if (validationResult.IsValid)
        {
            if (request.AccountFromId != request.AccountToId && request.CurrencyFromType != request.CurrencyToType)
            {
                var balance = GetBalanceByAccountId(request.AccountFromId);

                if (request.AmountFrom <= balance)
                {
                    TransactionDto transferWithdraw = new TransactionDto()
                    {
                        AccountId = request.AccountFromId,
                        TransactionType = Core.Enums.TransactionType.Transfer,
                        CurrencyType = request.CurrencyFromType,
                        Amount = request.AmountFrom * -1,
                        Date = request.Date
                    };

                    TransactionDto transferDeposit = new TransactionDto()
                    {
                        AccountId = request.AccountToId,
                        TransactionType = Core.Enums.TransactionType.Transfer,
                        CurrencyType = request.CurrencyToType,
                        Amount = request.AmountTo,
                        Date = request.Date
                    };

                    _transactionsRepository.AddTransferTransaction(transferWithdraw, transferDeposit);
                }

                throw new NotEnoughMoneyException("На балансе недостаточно средств для проведения операции.");
            }

            throw new NotFoundException("Нельзя сделать перевод внутри одного счёта. Операция не существует.");
        }

        else
        {
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
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
}