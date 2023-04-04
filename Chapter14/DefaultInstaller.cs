using System.Reflection;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Extensions.Logging;

namespace Chapter14;

public class DefaultInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(
            Component.For<IAuthenticator>()
                .ImplementedBy<Authenticator>()
                .LifestyleTransient());

        container.Register(
            Component.For<IAuthorizer>()
                .ImplementedBy<Authorizer>()
                .LifestyleTransient());

        container.Register(
            Component.For<IUsersService>()
                .ImplementedBy<UsersService>()
                .Proxy.AdditionalInterfaces(typeof(IAuthorizer), typeof(IAuthenticator))
                .Proxy.MixIns(_ => _
                    .Component<Authorizer>()
                    .Component<Authenticator>())
                .Interceptors<LoggingInterceptor>()
                .LifestyleTransient());

        container.Register(
            Component.For<IUsersServiceComposition>()
                .UsingFactoryMethod((kernel, context) =>
                {
                    var proxyGenerator = new ProxyGenerator();
                    var proxyGenerationOptions = new ProxyGenerationOptions();
                    proxyGenerationOptions.AddMixinInstance(container.Resolve<IAuthorizer>());
                    proxyGenerationOptions.AddMixinInstance(container.Resolve<IAuthenticator>());
                    var logger = container.Resolve<ILogger<UsersService>>();
                    proxyGenerationOptions.AddMixinInstance(new UsersService(logger));
                    var usersServiceComposition = (proxyGenerator.CreateClassProxyWithTarget(
                        typeof(object),
                        new[] { typeof(IUsersServiceComposition) },
                        new object(),
                        proxyGenerationOptions) as IUsersServiceComposition)!;
                    return usersServiceComposition;
                }));

        container.Register(Component.For<LoggingInterceptor>());
        container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetEntryAssembly())
            .Pick()
            .WithService.DefaultInterfaces()
            .Configure(_ => _.Interceptors<LoggingInterceptor>())
            .LifestyleTransient());
    }
}
