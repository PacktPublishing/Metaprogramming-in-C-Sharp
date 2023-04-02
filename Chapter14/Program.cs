using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Chapter14;
using Microsoft.Extensions.Logging;


var container = new WindsorContainer();
container.Install(FromAssembly.InThisApplication(Assembly.GetEntryAssembly()));

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
container.Register(Component.For<ILoggerFactory>().Instance(loggerFactory));

var createLoggerMethod = typeof(LoggerFactoryExtensions)
    .GetMethods(BindingFlags.Public | BindingFlags.Static)
    .First(_ => _.Name == nameof(LoggerFactory.CreateLogger) && _.IsGenericMethod);

container.Register(Component.For<ILogger>().UsingFactoryMethod((kernel, context) =>
{
    var loggerFactory = kernel.Resolve<ILoggerFactory>();
    return loggerFactory.CreateLogger(context.Handler.ComponentModel.Implementation);
}).LifestyleTransient());
container.Register(Component.For(typeof(ILogger<>)).UsingFactoryMethod((kernel, context) =>
{
    var loggerFactory = kernel.Resolve<ILoggerFactory>();
    var logger = createLoggerMethod.MakeGenericMethod(context.RequestedType.GenericTypeArguments[0]).Invoke(null, new[] { loggerFactory });
    return logger;
}));

var usersService = container.Resolve<IUsersService>();
var result = await usersService.Register("jane@doe.io", "Password1");

Console.ReadLine();