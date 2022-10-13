using System.ComponentModel;
using System.Globalization;

namespace Fundamentals;

public class ConceptAsTypeConverter<TConcept, TValue> : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(TValue) || sourceType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return ConceptFactory.CreateConceptInstance(typeof(TConcept), value);
    }
}
