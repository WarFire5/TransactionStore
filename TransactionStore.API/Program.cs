using MassTransit;
using Serilog;
using TransactionStore.API.Configuration;
using TransactionStore.API.Consumers;
using TransactionStore.API.Controllers;
using TransactionStore.API.Extensions;
using TransactionStore.Business;
using TransactionStore.Business.Services;
using TransactionStore.DataLayer;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    // Add services to the container.
    builder.Services.ConfigureApiServices(builder.Configuration);
    builder.Services.ConfigureBllServices();
    builder.Services.ConfigureDalServices();

    builder.Host.UseSerilog();

    //var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    //{
    //    cfg.ReceiveEndpoint("order-created-event", e =>
    //    {
    //        e.ConfigureConsumer<RatesInfoConsumer>(context);
    //    });
    //});

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();

    app.MapControllers();

    Log.Information("Running up");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex.Message);
}
finally
{
    Log.Information("App stopped.");
    Log.CloseAndFlush();
}