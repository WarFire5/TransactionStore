using Microsoft.AspNetCore.Mvc.Filters;

namespace TransactionStore.API.Filters;

public class GlobalFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        //здесь будет фильтрация по адресу нужного API
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        throw new NotImplementedException();
    }
}
