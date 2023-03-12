using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapter13.Commands;

public class CommandActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            var authorizationResult = context.HttpContext.GetAuthorizationResult();
            var isAuthorized = authorizationResult?.Succeeded ?? true;

            var exceptionMessages = new List<string>();
            var exceptionStackTrace = string.Empty;
            ActionExecutedContext? result = null;
            object? response = null;
            if (context.ModelState.IsValid && isAuthorized)
            {
                result = await next();

                if (result.Exception is not null)
                {
                    var exception = result.Exception;
                    exceptionStackTrace = exception.StackTrace;

                    do
                    {
                        exceptionMessages.Add(exception.Message);
                        exception = exception.InnerException;
                    }
                    while (exception is not null);

                    result.Exception = null!;
                }

                if (result.Result is ObjectResult objectResult)
                {
                    response = objectResult.Value;
                }
            }

            var commandResult = new CommandResult
            {
                CorrelationId = Guid.NewGuid(),
                IsAuthorized = isAuthorized,
                ValidationResults = context.ModelState.SelectMany(_ => _.Value!.Errors.Select(e => e.ToValidationResult(_.Key))),
                ExceptionMessages = exceptionMessages.ToArray(),
                ExceptionStackTrace = exceptionStackTrace ?? string.Empty,
                Response = response
            };

            if (!commandResult.IsAuthorized)
            {
                context.HttpContext.Response.StatusCode = 401;   // Forbidden: https://www.rfc-editor.org/rfc/rfc9110.html#name-401-unauthorized
            }
            else if (!commandResult.IsValid)
            {
                context.HttpContext.Response.StatusCode = 409;   // Conflict: https://www.rfc-editor.org/rfc/rfc9110.html#name-409-conflict
            }
            else if (commandResult.HasExceptions)
            {
                context.HttpContext.Response.StatusCode = 500;  // Internal Server error: https://www.rfc-editor.org/rfc/rfc9110.html#name-500-internal-server-error
            }

            var actualResult = new ObjectResult(commandResult);

            if (result is not null)
            {
                result.Result = actualResult;
            }
            else
            {
                context.Result = actualResult;
            }
        }
        else
        {
            await next();
        }
    }
}
