namespace EventSourcing;

public record AppendedEvent(EventContext Context, object Content);