using System.Reflection;
using Fundamentals;

var types = new Types();

var typesWithPII = types.All.Where(_ => _.GetMembers().Any(m => m.HasAttribute<PersonalIdentifiableInformationAttribute>()));
var typeNames = string.Join("\n", typesWithPII.Select(_ => _.FullName));
Console.WriteLine(typeNames);


// var commands = types.FindMultiple<ICommand>();
// var typeNames = string.Join("\n", commands.Select(_ => _.Name));
// Console.WriteLine(typeNames);

// Console.WriteLine("\n\nGDPR Report");
// var typesWithConcepts = types.All
//                             .SelectMany(_ =>
//                                 _.GetProperties()
//                                     .Where(p => p.PropertyType.IsPIIConcept()))
//                             .GroupBy(_ => _.DeclaringType);

// foreach (var typeWithConcepts in typesWithConcepts)
// {
//     Console.WriteLine($"Type: {typeWithConcepts.Key!.FullName}");
//     foreach (var property in typeWithConcepts)
//     {
//         Console.WriteLine($"  Property : {property.Name}");
//     }
// }
