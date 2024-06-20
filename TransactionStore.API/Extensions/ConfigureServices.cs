using MassTransit;
using TransactionStore.API.Consumers;
using TransactionStore.API.Filters;
using TransactionStore.Core.Data;
using TransactionStore.Core.Models;

namespace TransactionStore.API.Extensions;

public static class ConfigureServices
{
    public static void ConfigureApiServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddControllers(config =>
        {
            config.Filters.Add(new GlobalFilter());
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureDataBase(configurationManager);
        services.AddAutoMapper(typeof(TransactionsMappingProfile));
        services.AddMassTransit(x =>
        {
            x.AddConsumer<RatesInfoConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost");

                cfg.ReceiveEndpoint("currency_rates", e =>
                {
                    e.ConfigureConsumer<RatesInfoConsumer>(context);
                });
            });
        });
        services.AddScoped<ICurrencyRatesProvider, CurrencyRatesProvider>();
    }
}