using MassTransit;
using Messaging.Shared;
using Serilog;
using TransactionStore.Core.DTOs;

namespace TransactionStore.Business.Services;
public class MessagesService : IMessagesService
{
    private readonly ILogger _logger = Log.ForContext<MessagesService>();
    private readonly IPublishEndpoint _publishEndpoint;
    private Task _publish;

    public MessagesService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishTransactionAsync(List<TransactionDto> transactions, decimal comissionAmount)
    {
        foreach (var t in transactions)
        {
            var id = t.Id;
            var accountId = t.AccountId;
            var transactionType = t.TransactionType;
            var amount = t.Amount;
            var date = t.Date;

            _logger.Information("Sending transaction info to RabbitMQ. / Отправляем информацию о транзакции в RabbitMQ.");

            _publish = _publishEndpoint.Publish<TransactionCreated>(new
            {
                Id = id,
                AccountId = accountId,
                TransactionType = transactionType,
                Amount = amount,
                Comission = comissionAmount,
                Date = date,
            });
        }
            await Task.WhenAll(_publish);
    }
}