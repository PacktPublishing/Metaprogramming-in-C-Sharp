using System.Reflection;

namespace EventSourcing;

public class ObserverHandler
{
    readonly Dictionary<Type, IEnumerable<MethodInfo>> _methodsByEventType;
    readonly IServiceProvider _serviceProvider;
    readonly Type _targetType;

    public IEnumerable<Type> EventTypes => _methodsByEventType.Keys;

    public ObserverHandler(IServiceProvider serviceProvider, Type targetType)
    {
        _serviceProvider = serviceProvider;
        _targetType = targetType;

        _methodsByEventType = targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                        .Where(_ => IsObservingMethod(_))
                                        .GroupBy(_ => _.GetParameters()[0].ParameterType)
                                        .ToDictionary(_ => _.Key, _ => _.ToArray().AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task OnNext(AppendedEvent @event)
    {
        var eventType = @event.Content.GetType();

        if (_methodsByEventType.ContainsKey(eventType))
        {
            var actualObserver = _serviceProvider.GetService(_targetType);
            Task returnValue;
            foreach (var method in _methodsByEventType[eventType])
            {
                var parameters = method.GetParameters();

                if (parameters.Length == 2)
                {
                    returnValue = (Task)method.Invoke(actualObserver, new object[] { @event.Content, @event.Context })!;
                }
                else
                {
                    returnValue = (Task)method.Invoke(actualObserver, new object[] { @event.Content })!;
                }

                if (returnValue is not null) await returnValue;
            }
        }
    }

    bool IsObservingMethod(MethodInfo methodInfo)
    {
        var isObservingMethod = methodInfo.ReturnType.IsAssignableTo(typeof(Task)) ||
                                methodInfo.ReturnType == typeof(void);

        if (!isObservingMethod) return false;
        var parameters = methodInfo.GetParameters();
        if (parameters.Length >= 1)
        {
            isObservingMethod = parameters[0].ParameterType.IsAssignableTo(typeof(IEvent));
            if (parameters.Length == 2)
            {
                isObservingMethod &= parameters[1].ParameterType == typeof(EventContext);
            }
            else if (parameters.Length > 2)
            {
                isObservingMethod = false;
            }
            return isObservingMethod;
        }

        return false;
    }
}
