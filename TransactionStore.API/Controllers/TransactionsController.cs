using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
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

    // получаем баланс аккаунта по его Id
    [HttpGet("/balance/{id}")]
    public ActionResult<AccountBalanceResponse> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Получаем баланс аккаунта по его {id}");
        return Ok(_transactionsService.GetBalanceByAccountId(id));
    }

    // добавляем транзакцию на депозит
    [HttpPost("/deposit")]
    public ActionResult<Guid> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        return Ok(_transactionsService.AddDepositTransaction(request));
    }

    // добавляем транзакцию на снятие 
    [HttpPost("/withdraw")]
    public ActionResult<Guid> AddWithdrawTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"Для счёта с Id {request.AccountId} добавлена транзакция на снятие.");
        return Ok(_transactionsService.AddWithdrawTransaction(request));
    }

    // добавляем транзакцию на трансфер 
    [HttpPost("/transfer")]
    public ActionResult AddTransferTransaction([FromBody] TransferRequest request)
    {
        _logger.Information(
            $"Транзакция на перевод со счёта с Id {request.AccountFromId} на счёт с Id {request.AccountToId} добавлена в базу данных.");
        _transactionsService.AddTransferTransaction(request);
        return Ok();
    }
}