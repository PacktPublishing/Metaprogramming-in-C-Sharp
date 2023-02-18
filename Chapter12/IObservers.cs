
namespace Chapter12;

public interface IObservers
{
    IEnumerable<ObserverHandler> Handlers { get; }
}
