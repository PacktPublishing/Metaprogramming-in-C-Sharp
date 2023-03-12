using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Chapter13;

public class CrossCuttingAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        context.SetAuthorizationResult(authorizeResult);
        await _defaultHandler.HandleAsync(next, context, policy, PolicyAuthorizationResult.Success());
    }
}
