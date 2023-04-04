using Castle.DynamicProxy;

namespace Chapter14;

public class AuthorizationInterceptor : IInterceptor
{
    readonly IUsersServiceComposition _usersService;

    public AuthorizationInterceptor(IUsersServiceComposition usersService)
    {
        _usersService = usersService;
    }

    public void Intercept(IInvocation invocation)
    {
        if (_usersService.IsAuthorized("jane@doe.io", invocation.Method.Name))
        {
            invocation.Proceed();
        }
    }
}
