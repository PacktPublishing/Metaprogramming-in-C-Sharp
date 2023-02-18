using EventSourcing;

namespace Chapter12;

public class AccountBalance
{
    public Task DepositPerformed(DepositPerformed @event, EventContext context)
    {
        Console.WriteLine($"Deposit of {@event.Amount} performed on {context.EventSourceId}");
        return Task.CompletedTask;
    }

    public Task WithdrawalPerformed(WithdrawalPerformed @event, EventContext context)
    {
        Console.WriteLine($"Withdrawal of {@event.Amount} performed on {context.EventSourceId}");
        return Task.CompletedTask;
    }
}