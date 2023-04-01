using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Chapter14;

public class LoggingInterceptor : IInterceptor
{
    readonly ILoggerFactory _loggerFactory;

    public LoggingInterceptor(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public void Intercept(IInvocation invocation)
    {
        var logger = _loggerFactory.CreateLogger(invocation.TargetType)!;
        logger.BeforeInvocation(invocation.Method.Name);

        try
        {
            invocation.Proceed();

            if (invocation.ReturnValue is Task task)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.InvocationError(invocation.Method.Name, t.Exception!);
                    }
                    else
                    {
                        logger.AfterInvocation(invocation.Method.Name);
                    }
                });
            }
            else
            {
                logger.AfterInvocation(invocation.Method.Name);
            }
        }
        catch (Exception ex)
        {
            logger.InvocationError(invocation.Method.Name, ex);
            throw;
        }
    }
}
