using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Chapter14;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var createLoggerMethod = typeof(LoggerFactoryExtensions)
    .GetMethods(BindingFlags.Public | BindingFlags.Static)
    .First(_ => _.Name == nameof(LoggerFactory.CreateLogger) && _.IsGenericMethod);
var logger = loggerFactory.CreateLogger("Root");

var container = new WindsorContainer();
container.Register(Component.For<ILoggerFactory>().Instance(loggerFactory));
container.Register(Component.For<ILogger>().UsingFactoryMethod((kernel, context) =>
{
    var loggerFactory = kernel.Resolve<ILoggerFactory>();
    return loggerFactory.CreateLogger(context.RequestedType);
}));
container.Register(Component.For(typeof(ILogger<>)).UsingFactoryMethod((kernel, context) =>
{
    var loggerFactory = kernel.Resolve<ILoggerFactory>();
    var logger = createLoggerMethod.MakeGenericMethod(context.RequestedType.GenericTypeArguments[0]).Invoke(null, new[] { loggerFactory });
    return logger;
}));
container.Install(FromAssembly.InThisApplication(Assembly.GetEntryAssembly()));

var foo = container.Resolve<IUsersService>();
var result = await foo.Register("jane@doe.io", "Password1");

Console.ReadLine();