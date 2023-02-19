namespace EventSourcing;

public interface IEventLog
{
    Task Append(EventSourceId eventSourceId, IEvent @event);
}
