using System.Reflection;
using Castle.DynamicProxy;

namespace Chapter14;

public class InterceptorSelector : IInterceptorSelector
{
    public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
    {
        if (type.Namespace?.StartsWith("Chapter14.Todo", StringComparison.InvariantCulture) ?? false)
        {
            return interceptors;
        }

        return interceptors.Where(_ => _.GetType() != typeof(AuthorizationInterceptor)).ToArray();
    }
}