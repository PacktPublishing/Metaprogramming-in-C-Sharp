using System.Reflection;

var assembly = Assembly.GetEntryAssembly();
Console.WriteLine(assembly!.FullName);
var assemblies = assembly!.GetReferencedAssemblies();
var assemblyNames = string.Join(", ", assemblies.Select(_ => _.Name));
Console.WriteLine(assemblyNames);

