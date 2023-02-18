using EventSourcing;

namespace Chapter12;

public record WithdrawalPerformed(decimal Amount) : IEvent;
