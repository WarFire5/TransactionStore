using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Transactions.Requests;

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
    [HttpPost("/transaction/deposit")]
    public ActionResult<Guid> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"Для счёта с Id {request.AccountId} добавлена транзакция типа {request.TransactionType}.");
        return Ok(_transactionsService.AddDepositTransaction(request));
    }

    // добавляем транзакцию на снятие 
    [HttpPost("/transaction/withdraw")]
    public ActionResult<Guid> AddWithdrawTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(
            $"Для счёта с Id {request.AccountId} добавлена транзакция типа {request.TransactionType}.");
        return Ok(_transactionsService.AddWithdrawTransaction(request));
    }

    // добавляем транзакцию на трансфер 
    [HttpPost("/transaction/transfer")]
    public ActionResult AddTransferTransaction([FromBody] TransferRequest request)
    {
        _logger.Information(
            $"Транзакция типа {request.TransactionType} со счёта {request.AccountFromId} на счёт {request.AccountToId} добавлена.");
        _transactionsService.AddTransferTransaction(request);
        return Ok();
    }
}