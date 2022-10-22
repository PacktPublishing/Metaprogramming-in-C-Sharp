using System.Reflection;
using Fundamentals;

var types = new Types();

var piiTypes = types.All.Where(_ => _
                    .GetMembers()
                    .Any(m => m.HasAttribute<PersonalIdentifiableInformationAttribute>()));
var typeNames = string.Join("\n", piiTypes.Select(_ => _.FullName));
Console.WriteLine(typeNames);

Console.WriteLine("\n\nGDPR Report");
var typesWithPII = types.All
                        .SelectMany(_ =>
                            _.GetProperties()
                                .Where(p => p.HasAttribute<PersonalIdentifiableInformationAttribute>()))
                        .GroupBy(_ => _.DeclaringType);

foreach (var typeWithPII in typesWithPII)
{
    Console.WriteLine($"Type: {typeWithPII.Key!.FullName}");
    foreach (var property in typeWithPII)
    {
        var pii = property.GetCustomAttribute<PersonalIdentifiableInformationAttribute>();
        Console.WriteLine($"  Property : {property.Name}");
        Console.WriteLine($"    Reason : {pii!.ReasonForCollecting}");
    }
}
