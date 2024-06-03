using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.API.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountsController : Controller
{
    private readonly ITransactionsService _transactionsService;
    private readonly Serilog.ILogger _logger = Log.ForContext<AccountsController>();

    public AccountsController(ITransactionsService transactionsService)
    {
        _transactionsService = transactionsService;
    }

    // получаем баланс по accountId
    [HttpGet("balance/{id}")]
    public ActionResult<AccountBalanceResponse> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Getting the account balance by its Id {id}. / Получаем баланс аккаунта по его Id {id}.");
        return Ok(_transactionsService.GetBalanceByAccountId(id));
    }

    // получаем список транзакций по accountId
    [HttpGet("{id}/transactions")]
    public ActionResult<List<TransactionResponse>> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information($"Getting the account transactions by its Id {id}. / Получаем транзакции аккаунта по его Id {id}.");
        return Ok(_transactionsService.GetTransactionsByAccountId(id));
    }
}