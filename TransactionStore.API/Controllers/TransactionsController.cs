using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;

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

    // добавляем транзакцию на депозит
    [HttpPost("/deposit")]
    public ActionResult<Guid> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        return Ok(_transactionsService.AddDepositWithdrawTransaction(TransactionType.Deposit, request));
    }

    // добавляем транзакцию на снятие 
    [HttpPost("/withdraw")]
    public ActionResult<Guid> AddWithdrawTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"A withdraw transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на снятие.");
        return Ok(_transactionsService.AddDepositWithdrawTransaction(TransactionType.Withdraw, request));
    }

    // добавляем транзакцию на трансфер 
    [HttpPost("/transfer")]
    public ActionResult AddTransferTransaction([FromBody] TransferRequest request)
    {
        _logger.Information(
            $"A transfer transaction from an account with Id {request.AccountFromId} to an account with Id {request.AccountToId} has been added into the database. / Транзакция на перевод со счёта с Id {request.AccountFromId} на счёт с Id {request.AccountToId} добавлена в базу данных.");
        _transactionsService.AddTransferTransaction(request);
        return Ok();
    }

    // получаем список транзакций по accountId
    [HttpGet("/by-{accountId}")]
    public ActionResult<List<TransactionResponse>> GetTransactionsByAccountId(Guid accountId)
    {
        _logger.Information($"Getting the account transactions by accountId {accountId}. / Получаем список транзакций по accountId {accountId}.");
        return Ok(_transactionsService.GetTransactionsByAccountId(accountId));
    }
}