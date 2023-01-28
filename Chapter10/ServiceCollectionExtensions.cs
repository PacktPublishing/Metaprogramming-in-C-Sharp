using Fundamentals;

namespace Chapter10;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBindingsByConvention(this IServiceCollection services, ITypes types)
    {
        Func<Type, Type, bool> convention = (i, t) => i.Namespace == t.Namespace && i.Name == $"I{t.Name}";
        var conventionBasedTypes = types!.All.Where(_ =>
        {
            var interfaces = _.GetInterfaces();
            if (interfaces.Length > 0)
            {
                var conventionInterface = interfaces.SingleOrDefault(i => convention(i, _));
                if (conventionInterface != default)
                {
                    return types!.All.Count(type => type.HasInterface(conventionInterface)) == 1;
                }
            }
            return false;
        });

        foreach (var conventionBasedType in conventionBasedTypes)
        {
            var interfaceToBind = types.All.Single(_ => _.IsInterface && convention(_, conventionBasedType));
            if (services.Any(_ => _.ServiceType == interfaceToBind))
            {
                continue;
            }

            _ = conventionBasedType.HasAttribute<SingletonAttribute>() ?
                services.AddSingleton(interfaceToBind, conventionBasedType) :
                services.AddTransient(interfaceToBind, conventionBasedType);
        }

        return services;
    }
}