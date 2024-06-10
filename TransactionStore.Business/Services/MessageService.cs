using MassTransit;
using Messaging.Shared;
using Serilog;
using TransactionStore.Core.Models.Requests;

namespace TransactionStore.Business.Services;
public class MessageService(IPublishEndpoint publishEndpoint): IMessageService
{
    private readonly ILogger _logger = Log.ForContext<MessageService>();

    public async Task PublishDepositWithdrawTransactionAsync(DepositWithdrawRequest request)
    {
        await publishEndpoint.Publish<DepositTransactionCreated>(new
        {
            request.AccountId,
            request.Amount,
            request.CurrencyType
        });
    }

}