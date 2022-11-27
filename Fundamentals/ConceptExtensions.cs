// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Fundamentals;

public static class ConceptExtensions
{
    public static bool IsPIIConcept(this Type objectType)
    {
        return objectType.IsDerivedFromOpenGeneric(typeof(PIIConceptAs<>));
    }

    public static bool IsConcept(this Type objectType)
    {
        return objectType.IsDerivedFromOpenGeneric(typeof(ConceptAs<>));
    }

    public static bool IsConcept(this object instance)
    {
        return IsConcept(instance.GetType());
    }

    public static Type GetConceptValueType(this Type type)
    {
        return ConceptMap.GetConceptValueType(type);
    }

    public static object GetConceptValue(this object conceptObject)
    {
        if (!IsConcept(conceptObject)) throw new TypeIsNotAConcept(conceptObject.GetType());

        return ((dynamic)conceptObject).Value;
    }
}
