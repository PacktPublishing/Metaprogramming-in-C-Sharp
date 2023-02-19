using Chapter12;
using EventSourcing;
using Fundamentals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var types = new Types();
        services.AddSingleton<ITypes>(types);
        services.AddBindingsByConvention(types);
        services.AddSelfBinding(types);
    })
    .Build();

var eventLog = host.Services.GetRequiredService<IEventLog>();

var bankAccountId = EventSourceId.New();
eventLog.Append(bankAccountId, new BankAccountOpened("Jane Doe"));
eventLog.Append(bankAccountId, new DepositPerformed(100));
eventLog.Append(bankAccountId, new WithdrawalPerformed(32));
eventLog.Append(bankAccountId, new BankAccountClosed());
