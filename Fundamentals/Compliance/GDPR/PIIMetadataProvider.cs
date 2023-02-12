// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Fundamentals.Compliance.GDPR;

/// <summary>
/// Represents a metadata provider for PII.
/// </summary>
public class PersonalIdentifiableInformationMetadataProvider : ICanProvideComplianceMetadataForType, ICanProvideComplianceMetadataForProperty
{
    /// <inheritdoc/>
    public bool CanProvide(Type type) => type.Implements(typeof(IHoldPersonalIdentifiableInformation)) || type.GetCustomAttribute<PersonalIdentifiableInformationAttribute>() != default;

    /// <inheritdoc/>
    public bool CanProvide(PropertyInfo property) =>
        property.GetCustomAttribute<PersonalIdentifiableInformationAttribute>() != default ||
        property.DeclaringType?.GetCustomAttribute<PersonalIdentifiableInformationAttribute>() != default ||
        CanProvide(property.PropertyType);

    /// <inheritdoc/>
    public ComplianceMetadata Provide(Type type)
    {
        if (!CanProvide(type))
        {
            throw new NoComplianceMetadataForType(type);
        }

        return new ComplianceMetadata(ComplianceMetadataType.PII, type.GetComplianceMetadataDetails());
    }

    /// <inheritdoc/>
    public ComplianceMetadata Provide(PropertyInfo property)
    {
        if (!CanProvide(property))
        {
            throw new NoComplianceMetadataForProperty(property);
        }

        return new ComplianceMetadata(ComplianceMetadataType.PII, property.GetComplianceMetadataDetails());
    }
}
