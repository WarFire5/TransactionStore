using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransactionStore.Core.Models.Leads;
using TransactionStore.Core.Models.Accounts;
using TransactionStore.Core.Models.Transactions;

namespace TransactionStore.API.Extensions;

public static class ConfigureServices
{
    public static void ConfigureApiServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "TransactionStore",
                ValidAudience = "UI",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TransactionStore_TransactionStore_TransactionStore_superSecretKey@345"))
            };
        });

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.ConfigureDataBase(configurationManager);
        services.AddAutoMapper(typeof(LeadsMappingProfile), typeof(AccountsMappingProfile), typeof(TransactionsMappingProfile));
    }
}