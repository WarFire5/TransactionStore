using MassTransit;
using Messaging.Shared;
using Serilog;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.Business.Services;
public class MessagesService : IMessagesService
{
    private readonly ILogger _logger = Log.ForContext<MessagesService>();
    private readonly IPublishEndpoint _publishEndpoint;
    private Guid _id;
    private Guid _accountId;
    private TransactionType _transactionType;
    private decimal _amount;
    private DateTime _date;

    public MessagesService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishTransactionAsync(List<TransactionDto> transactions, decimal comissionAmount)
    {
        foreach (var t in transactions)
        {
            _id = t.Id;
            _accountId = t.AccountId;
            _transactionType = t.TransactionType;
            _amount = t.Amount;
            _date = t.Date;

            _logger.Information("Sending transaction info to RabbitMQ. / Отправляем информацию о транзакции в RabbitMQ.");

            await _publishEndpoint.Publish<TransactionCreated>(new
            {
                Id = _id,
                AccountId = _accountId,
                TransactionType = _transactionType,
                Amount = _amount,
                Comission = comissionAmount,
                Date = _date,
            });
        }
    }
}