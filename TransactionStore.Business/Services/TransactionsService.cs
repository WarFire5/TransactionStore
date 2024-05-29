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
        List<TransactionDto> transactions = _transactionsRepository.GetTransactionsByAccountId(id);
        var balance = transactions.Sum(t => t.Amount);

        _logger.Information("Считаем и передаем баланс");
        AccountBalanceResponse accountBalance = new AccountBalanceResponse()
        {
            AccountId = transactions[0].AccountId,
            Balance = balance,
            CurrencyType = transactions[0].CurrencyType
        };
       
        return accountBalance;
    }

    public List<TransactionsByAccountIdResponse> GetTransactionsByAccountId(Guid id) 
    {
        _logger.Information("Вызываем метод репозитория");
        List<TransactionDto> transactions = _transactionsRepository.GetTransactionsByAccountId(id);
        return _mapper.Map<List<TransactionsByAccountIdResponse>>(transactions);
    }

    public List<TransactionsByLeadIdResponse> GetTransactionsByLeadId(Guid id)
    {
        _logger.Information("Вызываем метод репозитория");
        List<TransactionDto> transactions = _transactionsRepository.GetTransactionsByLeadId(id);
        return _mapper.Map<List<TransactionsByLeadIdResponse>>(transactions);
    }
}