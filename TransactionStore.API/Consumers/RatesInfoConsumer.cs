using MassTransit;
using Messaging.Shared;
using Serilog;
using TransactionStore.Core.Data;

namespace TransactionStore.API.Consumers;

public class RatesInfoConsumer(ICurrencyRatesProvider currencyRatesProvider) : IConsumer<RatesInfo>
{
    private readonly Serilog.ILogger _logger = Log.ForContext<RatesInfoConsumer>();

    public async Task Consume(ConsumeContext<RatesInfo> context)
    {
        currencyRatesProvider.SetRates(context.Message);
        _logger.Information($"Getting currency rates: {context.Message.Rates} from Rates Provider");
    }
}