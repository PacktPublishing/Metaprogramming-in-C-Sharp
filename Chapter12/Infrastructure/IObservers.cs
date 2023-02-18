namespace EventSourcing;

public interface IObservers
{
    Task OnNext(AppendedEvent @event);
}
