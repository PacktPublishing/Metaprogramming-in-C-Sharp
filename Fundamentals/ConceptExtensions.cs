namespace Fundamentals;

public static class ConceptExtensions
{
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
