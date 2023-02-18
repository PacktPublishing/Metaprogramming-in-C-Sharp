using System.Reflection;
using Fundamentals;

namespace Chapter12;

[Singleton]
public class Observers : IObservers
{
    public IEnumerable<ObserverHandler> Handlers { get; }

    public Observers(ITypes types, IServiceProvider serviceProvider)
    {
        Handlers = types.All.Where(_ => _.HasAttribute<ObserverAttribute>())
                            .Select(_ =>
                            {
                                var observer = _.GetCustomAttribute<ObserverAttribute>()!;
                                return new ObserverHandler(serviceProvider, _);
                            });
    }
}


public record BankAccountOpened(string CustomerName) : IEvent;

public record DepositPerformed(decimal Amount) : IEvent;

public record WithdrawalPerformed(decimal Amount) : IEvent;

public record BankAccountClosed();

[Observer]
public class AccountLifecycle
{
    public Task Opened(BankAccountOpened @event)
    {
        Console.WriteLine($"Account opened for {@event.CustomerName}");
        return Task.CompletedTask;
    }

}