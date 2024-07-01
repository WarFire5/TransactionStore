using MassTransit;
using Messaging.Shared;
using Serilog;
using System.Text.Json;
using TransactionStore.Core.Data;

namespace TransactionStore.API.Consumers;

public class RatesInfoConsumer(ICurrencyRatesProvider currencyRatesProvider) : IConsumer<RatesInfo>
{
    private readonly Serilog.ILogger _logger = Log.ForContext<RatesInfoConsumer>();
    public async Task Consume(ConsumeContext<RatesInfo> context)
    {
        // Логирование содержимого context.Message
        var messageJson = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { WriteIndented = true });
        _logger.Information($"Consuming message: {messageJson}");

        // Установка курсов валют
        currencyRatesProvider.SetRates(context.Message);

        // Логирование полученных курсов валют
        _logger.Information($"Getting currency rates: {JsonSerializer.Serialize(context.Message.Rates, new JsonSerializerOptions { WriteIndented = true })} from Rates Provider");
    }
}