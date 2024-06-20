using MassTransit;
using Messaging.Shared;
using TransactionStore.Core.Data;

namespace TransactionStore.API.Consumers;

public class RatesInfoConsumer: IConsumer<RatesInfo>
{
    private readonly ICurrencyRatesProvider _currencyRatesProvider;

    public RatesInfoConsumer(ICurrencyRatesProvider currencyRatesProvider)
    {
        _currencyRatesProvider=currencyRatesProvider;
    }

    public async Task Consume(ConsumeContext<RatesInfo> context)
    {
        _currencyRatesProvider.SetRates(context.Message);
    }
}
