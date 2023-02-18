using EventSourcing;

namespace Chapter12;

[Observer]
public class AccountLifecycle
{
    public Task Opened(BankAccountOpened @event)
    {
        Console.WriteLine($"Account opened for {@event.CustomerName}");
        return Task.CompletedTask;
    }

    public Task Closed(BankAccountClosed @event, EventContext context)
    {
        Console.WriteLine($"Account with id {context.EventSourceId} closed");
        return Task.CompletedTask;
    }
}
