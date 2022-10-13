using System.Reflection;

#nullable disable

namespace Fundamentals;

public static class ConceptFactory
{
    public static object CreateConceptInstance(Type type, object value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var genericArgumentType = GetPrimitiveTypeConceptIsBasedOn(type);
        value = TypeConversion.Convert(genericArgumentType, value);
        var instance = Activator.CreateInstance(type, value);
        var valueProperty = type.GetTypeInfo().GetProperty(nameof(ConceptAs<object>.Value));
        valueProperty.SetValue(instance, value, null);
        return instance;
    }

    static Type GetPrimitiveTypeConceptIsBasedOn(Type conceptType)
    {
        return ConceptMap.GetConceptValueType(conceptType);
    }
}
