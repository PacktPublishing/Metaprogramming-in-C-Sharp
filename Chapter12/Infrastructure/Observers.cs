using System.Reflection;
using Fundamentals;

namespace EventSourcing;

[Singleton]
public class Observers : IObservers
{
    readonly IEnumerable<ObserverHandler> _handlers;

    public Observers(ITypes types, IServiceProvider serviceProvider)
    {
        _handlers = types.All.Where(_ => _.HasAttribute<ObserverAttribute>())
                            .Select(_ =>
                            {
                                var observer = _.GetCustomAttribute<ObserverAttribute>()!;
                                return new ObserverHandler(serviceProvider, _);
                            });
    }

    public Task OnNext(AppendedEvent @event)
    {
        var tasks = _handlers.Where(_ => _.EventTypes.Contains(@event.Content.GetType()))
                            .Select(_ => _.OnNext(@event));
        return Task.WhenAll(tasks);
    }
}
