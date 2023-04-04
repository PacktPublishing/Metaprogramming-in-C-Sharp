using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Chapter14;
using Microsoft.Extensions.Logging;


var container = new WindsorContainer();

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

container.Install(FromAssembly.InThisApplication(Assembly.GetEntryAssembly()));

var usersService = container.Resolve<IUsersService>();
var result = await usersService.Register("jane@doe.io", "Password1");

var authenticated = (usersService as IAuthenticator)!.Authenticate("jane@doe.io", "Password1");
var authorized = (usersService as IAuthorizer)!.IsAuthorized("jane@doe.io", "Some Action");
Console.WriteLine($"Authenticated: {authenticated}");
Console.WriteLine($"Authorized: {authorized}");

var composition = container.Resolve<IUsersServiceComposition>();
authenticated = composition.Authenticate("jane@doe.io", "Password1");
authorized = composition.IsAuthorized("jane@doe.io", "Some Action");
Console.WriteLine($"Authenticated: {authenticated}");
Console.WriteLine($"Authorized: {authorized}");

Console.ReadLine();