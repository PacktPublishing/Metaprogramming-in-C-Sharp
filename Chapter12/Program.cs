using Chapter12;
using Fundamentals;

var builder = WebApplication.CreateBuilder(args);
var types = new Types();
builder.Services.AddSingleton<ITypes>(types);
builder.Services.AddBindingsByConvention(types);
builder.Services.AddSelfBinding(types);

var app = builder.Build();

var @event = new AppendedEvent(
    new EventContext(EventSourceId.New(), 0, DateTimeOffset.UtcNow),
    new BankAccountOpened("John Doe"));
var observers = app.Services.GetRequiredService<IObservers>();
foreach (var handler in observers.Handlers)
{
    await handler.OnNext(@event);
}

app.MapGet("/", () => "Hello World!");

app.Run();
