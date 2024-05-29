using Microsoft.AspNetCore.Mvc;
using Serilog;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.API.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountsController: Controller
{
    private readonly ITransactionsService _transactionsService;
    private readonly Serilog.ILogger _logger = Log.ForContext<AccountsController>();

    public AccountsController(ITransactionsService transactionsService)
    {
        _transactionsService = transactionsService;
    }

    [HttpGet("/balance/{id}")]
    public ActionResult<AccountBalanceResponse> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Получаем баланс аккаунта {id}");
        return Ok(_transactionsService.GetBalanceByAccountId(id));
    }

    [HttpGet("/transactions/{id}")]
    public ActionResult<List<TransactionsByAccountIdResponse>> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information($"Получаем транзакции аккаунта {id}");
        return Ok(_transactionsService.GetTransactionsByAccountId(id));
    }
}
