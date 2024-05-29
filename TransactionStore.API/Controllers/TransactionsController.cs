using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
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

    [HttpGet("{id}")]
    public ActionResult<List<TransactionsByLeadIdResponse>> GetTransactionsByLeadId(Guid id)
    {
        _logger.Information($"Получаем транзакции лида {id}");
        return Ok(_transactionsService.GetTransactionsByLeadId(id));
    }
}
