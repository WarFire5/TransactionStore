using Serilog;
using System.Net;
using TransactionStore.Core.Exceptions;

namespace TransactionStore.API.Configuration;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger = Log.ForContext<ExceptionMiddleware>();

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ValidationException ex)
        {
            _logger.Error("Ошибка валидации / Validation error: {message}", ex.Message);
            await HandleValidationExceptionAsync(httpContext, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.Error("Контент не найден / Content not found error: {message}", ex.Message);
            await HandleNotFoundExceptionAsync(httpContext, ex);
        }
        catch (ServiceUnavailableException ex)
        {
            _logger.Error($"Ошибка {ex.Message}");
            await HandleServiceUnavailableExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.Error($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
        }.ToString());
    }

    private async Task HandleNotFoundExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
        }.ToString());
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
        }.ToString());
    }

    private async Task HandleServiceUnavailableExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    }
}