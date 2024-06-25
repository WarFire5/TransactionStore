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
            _publish = _publishEndpoint.Publish<TransactionCreated>(new
            {
                t.Id,
                t.AccountId,
                t.TransactionType,
                t.Amount,
                Comission=comissionAmount,
                t.Date,
            });

            _logger.Information("Sending transaction info to RabbitMQ. / Отправляем информацию о транзакции в RabbitMQ.");
        }
            await Task.WhenAll(_publish);
    }
}