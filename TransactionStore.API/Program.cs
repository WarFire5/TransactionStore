using Serilog;
using System.Text.Json;
using TransactionStore.API.Configuration;
using TransactionStore.API.Extensions;
using TransactionStore.Business;
using TransactionStore.DataLayer;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
    builder.Logging.ClearProviders();

    Log.Information("addJson");
    builder.Configuration.AddJsonFile("appsettings.DefaultConfiguration.json", optional: false, reloadOnChange: true);
    Log.Information("underRead");
    var conf = await builder.Configuration.ReadSettingsFromConfigurationManager();
    Log.Information("afterRead");
    var jsonMessage = JsonSerializer.Serialize(conf);
    Log.Information(jsonMessage);
    var a = builder.Configuration["DatabaseSettings:TransactionStoreDb"];
    Log.Information(a);
    var b= builder.Configuration["Serilog:WriteTo:1:Args:path"];
    Log.Information(b);

    builder.Services.ConfigureApiServices(builder.Configuration);
    builder.Services.ConfigureBllServices();
    builder.Services.ConfigureDalServices();

    builder.Host.UseSerilog();

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();

    app.MapControllers();

    Log.Information("Running up.");
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