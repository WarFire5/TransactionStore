using MassTransit;
using Messaging.Shared;
using Serilog;
using System.Text.Json;
using TransactionStore.API.Extensions;

namespace TransactionStore.API.Consumers;

public class SettingsConsumer(IConfiguration configuration) : IConsumer<ConfigurationMessage>
{
    private readonly Serilog.ILogger _logger = Log.ForContext<SettingsConsumer>();
    public async Task Consume(ConsumeContext<ConfigurationMessage> context)
    {
        var jsonMessage = JsonSerializer.Serialize(context.Message.Configurations);
        _logger.Information($"Consuming message: {jsonMessage}.");
        configuration.UpdateSettingsFromConfigurationManager(context.Message.Configurations);
    }
}
