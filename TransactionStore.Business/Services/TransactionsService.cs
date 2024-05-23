using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
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

    public CurrencyTypeResponse GetCurrencyTypeByAccountId(Guid id)
    {
        var сurrencyType = _transactionsRepository.GetCurrencyTypeByAccountId(id);

        //if (сurrencyType == null)
        //{
        //    throw new NotFoundException("Операции с переданным accountId не найдены.");
        //}

        return _mapper.Map<CurrencyTypeResponse>(сurrencyType);
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

    public Guid AddDepositTransaction(DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

        if (validationResult.IsValid)
        {
            var сurrencyTypeResponse = GetCurrencyTypeByAccountId(request.AccountId);

            if (сurrencyTypeResponse == null) сurrencyTypeResponse = new CurrencyTypeResponse();

            if (request.CurrencyType == сurrencyTypeResponse.CurrencyType || сurrencyTypeResponse.CurrencyType == null)
            {
                TransactionDto transaction = new TransactionDto()
                {
                    AccountId = request.AccountId,
                    TransactionType = TransactionType.Deposit,
                    CurrencyType = request.CurrencyType,
                    Amount = request.Amount,
                    Date = request.Date
                };

                return _transactionsRepository.AddDepositTransaction(transaction);
            }

            throw new InappropriateCurrencyTypeException("Тип валюты не совпадает с типом валюты аккаунта.");
        }

        string exceptions = string.Join(Environment.NewLine, validationResult.Errors);
        throw new ValidationException(exceptions);
    }

    public Guid AddWithdrawTransaction(DepositWithdrawRequest request)
    {
        var validationResult = _addDepositWithdrawValidator.Validate(request);

        if (validationResult.IsValid)
        {
            var сurrencyTypeResponse = GetCurrencyTypeByAccountId(request.AccountId);

            if (сurrencyTypeResponse == null) сurrencyTypeResponse = new CurrencyTypeResponse();

            if (request.CurrencyType == сurrencyTypeResponse.CurrencyType || сurrencyTypeResponse.CurrencyType == null)
            {
                var accountBalanceResponse = GetBalanceByAccountId(request.AccountId);

                if (request.Amount <= accountBalanceResponse.Balance)
                {
                    TransactionDto transaction = new TransactionDto()
                    {
                        AccountId = request.AccountId,
                        TransactionType = TransactionType.Withdraw,
                        CurrencyType = request.CurrencyType,
                        Amount = request.Amount * -1,
                        Date = request.Date
                    };

                    return _transactionsRepository.AddWithdrawTransaction(transaction);
                }

                throw new NotEnoughMoneyException("На балансе недостаточно средств для проведения операции.");
            }

            throw new InappropriateCurrencyTypeException("Тип валюты не совпадает с типом валюты аккаунта.");
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
                var сurrencyTypeResponse = GetCurrencyTypeByAccountId(request.AccountFromId);

                if (сurrencyTypeResponse == null) сurrencyTypeResponse = new CurrencyTypeResponse();

                if (request.CurrencyFromType == сurrencyTypeResponse.CurrencyType || сurrencyTypeResponse.CurrencyType == null)
                {
                    var accountBalanceResponse = GetBalanceByAccountId(request.AccountFromId);

                    if (request.Amount <= accountBalanceResponse.Balance)
                    {
                        var сurrencyTypeToResponse = GetCurrencyTypeByAccountId(request.AccountToId);

                        if (сurrencyTypeToResponse == null) сurrencyTypeToResponse = new CurrencyTypeResponse();

                        if (request.CurrencyToType == сurrencyTypeResponse.CurrencyType || сurrencyTypeToResponse.CurrencyType == null)
                        {
                            TransactionDto transferWithdraw = new TransactionDto()
                            {
                                AccountId = request.AccountFromId,
                                TransactionType = TransactionType.Transfer,
                                CurrencyType = request.CurrencyFromType,
                                Amount = request.Amount * -1,
                                Date = request.Date
                            };

                            DictionaryOfCoefficients dictionaryOfCoefficients = new DictionaryOfCoefficients();
                            var coef = dictionaryOfCoefficients.GetRate(request.CurrencyFromType.ToString(), request.CurrencyToType.ToString());

                            TransactionDto transferDeposit = new TransactionDto()
                            {
                                AccountId = request.AccountToId,
                                TransactionType = TransactionType.Transfer,
                                CurrencyType = request.CurrencyToType,
                                Amount = request.Amount * coef,
                                Date = request.Date
                            };

                            _transactionsRepository.AddTransferTransaction(transferWithdraw, transferDeposit);
                        }

                        else
                        {
                            throw new InappropriateCurrencyTypeException("Тип валюты не совпадает с типом валюты аккаунта.");
                        }
                    }

                    else
                    {
                        throw new NotEnoughMoneyException("На балансе недостаточно средств для проведения операции.");
                    }
                }

                else
                {
                    throw new InappropriateCurrencyTypeException("Тип валюты не совпадает с типом валюты аккаунта.");
                }
            }

            else
            {
                throw new NotFoundException("Нельзя сделать перевод внутри одного счёта. Операция не существует.");
            }
        }

        else
        {
            string exceptions = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(exceptions);
        }
    }
}