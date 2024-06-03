using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Responses;

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

    [HttpGet("{id}/balance")]
    public async Task<ActionResult<AccountBalanceResponse>> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Getting the account balance by its Id {id}. / Получаем баланс аккаунта по его Id {id}.");
        var balance = await _transactionsService.GetBalanceByAccountIdAsync(id);
        return Ok(balance);
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information($"Getting the account transactions by its Id {id}. / Получаем транзакции аккаунта по его Id {id}.");
        var transactions = await _transactionsService.GetTransactionsByAccountIdAsync(id);
        return Ok(transactions);
    }
}