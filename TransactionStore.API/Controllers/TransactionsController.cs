using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.API.Controllers;

[ApiController]
[Route("/api/transactions")]
public class TransactionsController : Controller
{
    private readonly ITransactionsService _transactionsService;
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();

    public TransactionsController(ITransactionsService transactionsService)
    {
        _transactionsService = transactionsService;
    }

    [HttpPost("deposit")]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        var transactionId = await _transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);
        return Ok(transactionId);
    }

    [HttpPost("withdraw")]
    public async Task<ActionResult<Guid>> AddWithdrawTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"A withdraw transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на снятие.");
        var transactionId = await _transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, request);
        return Ok(transactionId);
    }

    [HttpPost("transfer")]
    public async Task<ActionResult> AddTransferTransaction([FromBody] TransferRequest request)
    {
        _logger.Information(
            $"A transfer transaction from an account with Id {request.AccountFromId} to an account with Id {request.AccountToId} has been added into the database. / Транзакция на перевод со счёта с Id {request.AccountFromId} на счёт с Id {request.AccountToId} добавлена в базу данных.");
        await _transactionsService.AddTransferTransactionAsync(request);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<TransactionWithAccountIdResponse>>> GetTransactionsById(Guid id)
    {
        _logger.Information($"Getting the account transactions by Id {id}. / Получаем список транзакций по Id {id}.");
        var transactions = await _transactionsService.GetTransactionsByIdAsync(id);
        return Ok(transactions);
    }
}