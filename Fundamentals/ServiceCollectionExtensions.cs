using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Fundamentals;

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

    public static IServiceCollection AddSelfBinding(this IServiceCollection services, ITypes types)
    {
        const TypeAttributes staticType = TypeAttributes.Abstract | TypeAttributes.Sealed;

        types.All.Where(_ =>
            (_.Attributes & staticType) != staticType &&
            !_.IsInterface &&
            !_.IsAbstract &&
            services.Any(s => s.ServiceType != _)).ToList().ForEach(_ =>
        {
            var __ = _.HasAttribute<SingletonAttribute>() ?
                services.AddSingleton(_, _) :
                services.AddTransient(_, _);
        });

        return services;
    }
}
