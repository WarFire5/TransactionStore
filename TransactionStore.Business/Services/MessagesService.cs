using MassTransit;
using Messaging.Shared;
using Serilog;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.Business.Services;
public class MessagesService(IPublishEndpoint publishEndpoint) : IMessagesService
{
    private readonly ILogger _logger = Log.ForContext<MessagesService>();
    private Task _publish;

    public async Task PublishTransactionAsync(List<TransactionDto> transactions, decimal comissionAmount, Currency currency)
    {
        foreach (var t in transactions)
        {
            _publish = publishEndpoint.Publish<TransactionCreated>(new
            {
                t.Id,
                t.AccountId,
                t.TransactionType,
                t.Amount,
                Currency=currency,
                Comission = comissionAmount,
                t.Date,
            });

            _logger.Information("Sending transaction info to RabbitMQ. / Отправляем информацию о транзакции в RabbitMQ.");
        }
        await _publish;
    }
}