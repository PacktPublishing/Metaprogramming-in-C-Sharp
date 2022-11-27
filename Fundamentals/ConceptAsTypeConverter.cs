// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
