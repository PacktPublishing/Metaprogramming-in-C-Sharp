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
