using Chapter11;
using Fundamentals;
using Fundamentals.Compliance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var types = new Types();
        services.AddSingleton<ITypes>(types);
        services.AddBindingsByConvention(types);
        services.AddSelfBinding(types);
    })
    .Build();

var complianceMetadataResolver = host.Services.GetRequiredService<IComplianceMetadataResolver>();

var typeToCheck = typeof(Patient);

Console.WriteLine($"Checking type for compliance rules: {typeToCheck.FullName}");

if (complianceMetadataResolver.HasMetadataFor(typeToCheck))
{
    var metadata = complianceMetadataResolver.GetMetadataFor(typeToCheck);
    foreach (var item in metadata)
    {
        Console.WriteLine($"Type level - {item.Details}");
    }
}

Console.WriteLine("");

foreach (var property in typeof(Patient).GetProperties())
{
    if (complianceMetadataResolver.HasMetadataFor(property))
    {
        var metadata = complianceMetadataResolver.GetMetadataFor(property);
        foreach (var item in metadata)
        {
            Console.WriteLine($"Property: {property.Name} - {item.Details}");
        }
    }
    else if (complianceMetadataResolver.HasMetadataFor(property.PropertyType))
    {
        var metadata = complianceMetadataResolver.GetMetadataFor(property.PropertyType);
        foreach (var item in metadata)
        {
            Console.WriteLine($"Property: {property.Name} - {item.Details}");
        }
    }
    else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().IsAssignableTo(typeof(IEnumerable<>)))
    {
        var type = property.PropertyType.GetGenericArguments().First();
        if (complianceMetadataResolver.HasMetadataFor(type))
        {
            Console.WriteLine($"\nProperty {property.Name} is a collection of type {type.FullName} with type level metadata");

            var metadata = complianceMetadataResolver.GetMetadataFor(type);
            foreach (var item in metadata)
            {
                Console.WriteLine($"{property.Name} - {item.Details}");
            }
        }
    }
}

