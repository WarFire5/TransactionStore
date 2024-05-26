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
        _logger.Information($"Получаем баланс аккаунта по его Id {id}.");
        return Ok(_transactionsService.GetBalanceByAccountId(id));
    }

    // добавляем транзакцию на депозит
    [HttpPost("/deposit")]
    public ActionResult<Guid> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        if (request == null || request.AccountId == Guid.Empty)
        {
            _logger.Warning("Received a null or invalid request in 'AddDepositTransaction' method. / Получен нулевой или недопустимый запрос в методе 'AddDepositTransaction'.");
            return BadRequest();
        }

        _logger.Information(
            $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        return Ok(_transactionsService.AddDepositWithdrawTransaction(request));
    }


    // добавляем транзакцию на снятие 
    [HttpPost("/withdraw")]
    public ActionResult<Guid> AddWithdrawTransaction([FromBody] DepositWithdrawRequest request)
    {
        if (request == null || request.AccountId == Guid.Empty)
        {
            _logger.Warning("Received a null or invalid request in 'AddWithdrawTransaction' method. / Получен нулевой или недопустимый запрос в методе 'AddWithdrawTransaction'.");
            return BadRequest();
        }

        _logger.Information(
            $"A withdraw transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на снятие.");
        return Ok(_transactionsService.AddDepositWithdrawTransaction(request));
    }

    // добавляем транзакцию на трансфер 
    [HttpPost("/transfer")]
    public ActionResult AddTransferTransaction([FromBody] TransferRequest request)
    {
        if (request == null || request.AccountFromId == Guid.Empty)
        {
            _logger.Warning("Received a null or invalid request in 'AddTransferTransaction' method. / Получен нулевой или недопустимый запрос в методе 'AddTransferTransaction'.");
            return BadRequest();
        }

        _logger.Information(
            $"A transfer transaction from an account with Id {request.AccountFromId} to an account with Id {request.AccountToId} has been added into the database. / Транзакция на перевод со счёта с Id {request.AccountFromId} на счёт с Id {request.AccountToId} добавлена в базу данных.");
        _transactionsService.AddTransferTransaction(request);
        return Ok();
    }
}