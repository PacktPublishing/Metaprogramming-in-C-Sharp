using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Fundamentals;

public class Types : ITypes
{
    readonly IContractToImplementorsMap _contractToImplementorsMap = new ContractToImplementorsMap();

    public Types(params string[] assemblyPrefixesToInclude)
    {
        All = DiscoverAllTypes(assemblyPrefixesToInclude);
        _contractToImplementorsMap.Feed(All);
    }

    public IEnumerable<Type> All { get; }

    IEnumerable<Type> DiscoverAllTypes(IEnumerable<string> assemblyPrefixesToInclude)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var dependencyModel = DependencyContext.Load(entryAssembly);
        var projectReferencedAssemblies = dependencyModel.RuntimeLibraries
                            .Where(_ => _.Type.Equals("project"))
                            .Select(_ => Assembly.Load(_.Name))
                            .ToArray();

        var assemblies = dependencyModel.RuntimeLibraries
                            .Where(_ => _.RuntimeAssemblyGroups.Count > 0 &&
                                        assemblyPrefixesToInclude.Any(asm => _.Name.StartsWith(asm)))
                            .Select(_ =>
                            {
                                try
                                {
                                    return Assembly.Load(_.Name);
                                }
                                catch
                                {
                                    return null!;
                                }
                            })
                            .Where(_ => _ is not null)
                            .Distinct()
                            .ToList();

        assemblies.AddRange(projectReferencedAssemblies);
        return assemblies.SelectMany(_ => _.GetTypes()).ToArray();
    }

    public Type FindSingle<T>() => FindSingle(typeof(T));

    public IEnumerable<Type> FindMultiple<T>() => FindMultiple(typeof(T));

    public Type FindSingle(Type type)
    {
        var typesFound = _contractToImplementorsMap.GetImplementorsFor(type);
        ThrowIfMultipleTypesFound(type, typesFound);
        return typesFound.SingleOrDefault()!;
    }

    public IEnumerable<Type> FindMultiple(Type type) => _contractToImplementorsMap.GetImplementorsFor(type);

    public Type FindTypeByFullName(string fullName)
    {
        var typeFound = _contractToImplementorsMap.All.SingleOrDefault(t => t.FullName == fullName);
        ThrowIfTypeNotFound(fullName, typeFound!);
        return typeFound!;
    }

    void ThrowIfMultipleTypesFound(Type type, IEnumerable<Type> typesFound)
    {
        if (typesFound.Count() > 1)
        {
            throw new MultipleTypesFound(type, typesFound);
        }
    }

    void ThrowIfTypeNotFound(string fullName, Type typeFound)
    {
        if (typeFound == null)
        {
            throw new UnableToResolveTypeByName(fullName);
        }
    }
}
