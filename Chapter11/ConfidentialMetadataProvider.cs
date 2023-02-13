using System.Reflection;
using Fundamentals.Compliance;

namespace Chapter11;

public class ConfidentialMetadataProvider : ICanProvideComplianceMetadataForType
{
    public bool CanProvide(Type type) => type.GetCustomAttribute<ConfidentialAttribute>() != null;

    public ComplianceMetadata Provide(Type type) => new("8dd1709a-bbe1-4b98-84e1-9e7be2fd4912", "The data is confidential");
}