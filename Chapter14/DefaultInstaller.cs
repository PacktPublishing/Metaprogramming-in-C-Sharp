using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Chapter14;

public class DefaultInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(Component.For<LoggingInterceptor>());
        container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetEntryAssembly())
            .Pick()
            .WithService.DefaultInterfaces()
            .Configure(_ => _.Interceptors<LoggingInterceptor>())
            .LifestyleTransient());
    }
}