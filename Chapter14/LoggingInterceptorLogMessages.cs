using Microsoft.Extensions.Logging;

namespace Chapter14;

internal static partial class LoggingInterceptorLogMessages
{
    [LoggerMessage(1, LogLevel.Information, "Before invoking {Method}", EventName = "BeforeInvocation")]
    internal static partial void BeforeInvocation(this ILogger logger, string method);

    [LoggerMessage(2, LogLevel.Error, "Error invoking {Method}", EventName = "InvocationError")]
    internal static partial void InvocationError(this ILogger logger, string method, Exception exception);

    [LoggerMessage(3, LogLevel.Information, "Before invoking {Method}", EventName = "AfterInvocation")]
    internal static partial void AfterInvocation(this ILogger logger, string method);
}