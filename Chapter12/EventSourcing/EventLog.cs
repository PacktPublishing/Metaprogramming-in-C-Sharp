namespace EventSourcing;

public class EventLog : IEventLog
{
    readonly IObservers _observers;
    EventSequenceNumber _sequenceNumber = 0;

    public EventLog(IObservers observers)
    {
        _observers = observers;
    }

    public async Task Append(EventSourceId eventSourceId, IEvent @event)
    {
        await _observers.OnNext(
            @event,
            new EventContext(eventSourceId, _sequenceNumber, DateTimeOffset.UtcNow));
        _sequenceNumber++;
    }
}