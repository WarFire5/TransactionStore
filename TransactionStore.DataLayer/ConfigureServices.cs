using Microsoft.Extensions.DependencyInjection;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.DataLayer;

public static class ConfigureServices
{
    public static void ConfigureDalServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
    }
}