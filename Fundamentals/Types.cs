using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Fundamentals;

public class Types
{
    public Types(params string[] assemblyPrefixesToInclude)
    {
        All = DiscoverAllTypes(assemblyPrefixesToInclude);
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
}
