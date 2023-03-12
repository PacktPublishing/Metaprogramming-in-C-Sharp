using Microsoft.AspNetCore.Authorization.Policy;

namespace Chapter13;

public static class HttpContextExtensions
{
    const string AuthorizeResultKey = "_AuthorizeResult";

    public static PolicyAuthorizationResult? GetAuthorizationResult(this HttpContext context) => (context.Items[AuthorizeResultKey] as PolicyAuthorizationResult)!;

    public static void SetAuthorizationResult(this HttpContext context, PolicyAuthorizationResult result) => context.Items[AuthorizeResultKey] = result;
}