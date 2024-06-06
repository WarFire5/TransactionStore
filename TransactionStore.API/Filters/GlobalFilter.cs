using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using TransactionStore.Core.Exceptions;
using ILogger = Serilog.ILogger;

namespace TransactionStore.API.Filters;

public class GlobalFilter : IActionFilter
{
    private readonly ILogger _logger = Log.ForContext<GlobalFilter>();
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (Convert.ToString(context.HttpContext.Request.Host) != "194.87.210.5:10000")
        {
            _logger.Debug($"Access from adress {context.HttpContext.Request.Host} denied.");
            throw new ForbiddenException();
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
}