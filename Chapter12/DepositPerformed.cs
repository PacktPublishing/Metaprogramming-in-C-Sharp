using EventSourcing;

namespace Chapter12;

public record DepositPerformed(decimal Amount) : IEvent;
