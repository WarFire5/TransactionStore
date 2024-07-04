using MassTransit;
using Messaging.Shared;
using Serilog;
using System.Text.Json;

namespace TransactionStore.API.Consumers;

public class SettingsConsumer : IConsumer<ConfigurationMessage>
{
    private readonly Serilog.ILogger _logger = Log.ForContext<SettingsConsumer>();
    public async Task Consume(ConsumeContext<ConfigurationMessage> context)
    {
        var message = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { WriteIndented = true });
        _logger.Information($"Consuming message: {message}.");
    }
}
