using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using TransactionStore.Core.Exceptions;
using ILogger = Serilog.ILogger;

namespace TransactionStore.API.Filters;

public class GlobalFilter(IConfiguration configuration) : IActionFilter
{
    private readonly ILogger _logger = Log.ForContext<GlobalFilter>();

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //if (context.HttpContext.Request.Host.ToString() != configuration["ServicesUrlSettings:Crm"])
        //{
        //    _logger.Debug($"Access from adress {context.HttpContext.Request.Host} denied.");
        //    throw new ForbiddenException($"Access from adress {context.HttpContext.Request.Host} denied.");
        //}
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
}