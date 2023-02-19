using EventSourcing;

namespace Chapter12;

public record BankAccountOpened(string CustomerName) : IEvent;
public record BankAccountClosed() : IEvent;
public record DepositPerformed(decimal Amount) : IEvent;
public record WithdrawalPerformed(decimal Amount) : IEvent;
