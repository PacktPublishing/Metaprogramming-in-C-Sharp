// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fundamentals;

public class ConceptAsJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsConcept();

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(ConceptAsJsonConverter<>).MakeGenericType(typeToConvert);
        return (Activator.CreateInstance(converterType) as JsonConverter)!;
    }
}
