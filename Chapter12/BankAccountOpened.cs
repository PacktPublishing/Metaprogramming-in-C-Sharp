using EventSourcing;

namespace Chapter12;

public record BankAccountOpened(string CustomerName) : IEvent;
