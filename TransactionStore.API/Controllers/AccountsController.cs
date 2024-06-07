using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.API.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountsController(ITransactionsService transactionsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<AccountsController>();

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information($"Getting the account transactions by its Id {id}. / Получаем транзакции аккаунта по его Id {id}.");
        var transactions = await transactionsService.GetTransactionsByAccountIdAsync(id);
        return Ok(transactions);
    }

    [HttpGet("{id}/balance")]
    public async Task<ActionResult<AccountBalanceResponse>> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Getting the account balance by its Id {id}. / Получаем баланс аккаунта по его Id {id}.");
        var balance = await transactionsService.GetBalanceByAccountIdAsync(id);
        return Ok(balance);
    }
}