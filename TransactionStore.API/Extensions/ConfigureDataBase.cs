using Microsoft.EntityFrameworkCore;
using Npgsql;
using TransactionStore.Core.Enums;
using TransactionStore.DataLayer;

namespace TransactionStore.API.Extensions;

public static class DataBaseExtensions
{
    public static void ConfigureDataBase(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var connectionString = configurationManager.GetConnectionString("TsConnection");

        var dataSourceBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var dataSource = dataSourceBuilder.ConnectionString;

        services.AddDbContext<TransactionStoreContext>(
            options => options
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention()
        );

        NpgsqlConnection.GlobalTypeMapper.MapEnum<CurrencyType>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<TransactionType>();
    }

    public static async Task MigrateAndReloadPostgresTypesAsync(this IServiceProvider serviceProvider, CancellationToken token = default)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TransactionStoreContext>();

            await dbContext.Database.MigrateAsync(token);

            if (dbContext.Database.GetDbConnection() is NpgsqlConnection npgsqlConnection)
            {
                await npgsqlConnection.OpenAsync(token);

                try
                {
                    await npgsqlConnection.ReloadTypesAsync();
                }
                finally
                {
                    await npgsqlConnection.CloseAsync();
                }
            }
        }
    }
}