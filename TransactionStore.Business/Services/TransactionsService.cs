using AutoMapper;
using FluentValidation;
using Serilog;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.Business.Services;

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly ILogger _logger = Log.ForContext<TransactionsService>();
    private readonly IMapper _mapper;
    private readonly IValidator<AddTransactionRequest> _addTransactionValidator;

    public TransactionsService(ITransactionsRepository usersRepository, IMapper mapper
        , IValidator<AddTransactionRequest> addTransactionValidator
        )
    {
        _transactionsRepository = usersRepository;
        _mapper = mapper;
        _addTransactionValidator = addTransactionValidator;
    }
}