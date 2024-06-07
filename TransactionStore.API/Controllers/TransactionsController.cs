using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.API.Controllers;

[ApiController]
[Route("/api/transactions")]
public class TransactionsController(ITransactionsService transactionsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();

    [HttpPost("deposit")]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        var transactionId = await transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);
        return Created($"/api/transactions/{transactionId}", transactionId);
    }

    [HttpPost("withdraw")]
    public async Task<ActionResult<Guid>> AddWithdrawTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"A withdraw transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на снятие.");
        var transactionId = await transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, request);
        return Created($"/api/transactions/{transactionId}", transactionId);
    }

    [HttpPost("transfer")]
    public async Task<ActionResult> AddTransferTransaction([FromBody] TransferRequest request)
    {
        _logger.Information(
            $"A transfer transaction from an account with Id {request.AccountFromId} to an account with Id {request.AccountToId} has been added into the database. / Транзакция на перевод со счёта с Id {request.AccountFromId} на счёт с Id {request.AccountToId} добавлена в базу данных.");
        await transactionsService.AddTransferTransactionAsync(request);
        return StatusCode(201);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<TransactionWithAccountIdResponse>>> GetTransactionById(Guid id)
    {
        _logger.Information($"Getting transaction by Id {id}. / Получаем транзакцию по Id {id}.");
        var transactions = await transactionsService.GetTransactionByIdAsync(id);
        return Ok(transactions);
    }
}