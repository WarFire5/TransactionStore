using Backend.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Transactions.Requests;

namespace TransactionStore.Business;

public static class ConfigureServices
{
    public static void ConfigureBllServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionsService, TransactionsService>();

        services.AddScoped<IValidator<AddTransactionRequest>, AddTransactionValidator>();
    }
}
