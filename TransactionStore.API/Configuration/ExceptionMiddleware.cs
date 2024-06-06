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
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.UnprocessableEntity, "Ошибка валидации. / Validation error.");
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound, "Контент не найден. / Content not found error.");
        }
        catch (ServiceUnavailableException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.ServiceUnavailable, "Нет соединения с базой данных. / There is no connection to the database.");
        }
        catch (ForbiddenException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.Forbidden, "Доступ запрещен. /Access denied.");
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, $"Something went wrong.");
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string logMessage)
    {
        _logger.Error($"{logMessage}: {exception.Message}");
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    }
}