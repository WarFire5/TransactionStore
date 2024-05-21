using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.Business.Services;

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();
    private readonly IMapper _mapper;
    private readonly IValidator<AddTransactionRequest> _addTransactionValidator;

    public TransactionsService(ITransactionsRepository usersRepository, IMapper mapper
           , IValidator<AddTransactionRequest> addTransactionValidator)
    {
        _transactionsRepository = usersRepository;
        _mapper = mapper;
        _addTransactionValidator = addTransactionValidator;
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