using System.Dynamic;
using NJsonSchema;

namespace Chapter9;

public class JsonSchemaType : DynamicObject
{
    readonly IDictionary<string, object?> _values = new Dictionary<string, object?>();
    readonly JsonSchema _schema;

    public JsonSchemaType(JsonSchema schema)
    {
        _schema = schema;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (!_schema.ActualProperties.ContainsKey(binder.Name))
        {
            return false;
        }
        ValidateType(binder.Name, value);
        _values[binder.Name] = value;
        return true;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (!_schema.ActualProperties.ContainsKey(binder.Name))
        {
            result = null!;
            return false;
        }

        result = _values.ContainsKey(binder.Name) ? result = _values[binder.Name] : result = null!;
        return true;
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type.Equals(typeof(Dictionary<string, object?>)))
        {
            var returnValue = new Dictionary<string, object?>(_values);
            var missingProperties = _schema.ActualProperties.Where(_ => !_values.Any(kvp => _.Key == kvp.Key));
            foreach (var property in missingProperties)
            {
                object defaultValue = property.Value.Type switch
                {
                    JsonObjectType.Array => Enumerable.Empty<object>(),
                    JsonObjectType.Boolean => false,
                    JsonObjectType.Integer => 0,
                    JsonObjectType.Number => 0,
                    _ => null!
                };

                returnValue[property.Key] = defaultValue;
            }
            result = returnValue;
            return true;
        }
        return base.TryConvert(binder, out result);
    }

    public override IEnumerable<string> GetDynamicMemberNames() => _schema.ActualProperties.Keys;



    void ValidateType(string property, object? value)
    {
        if (value is not null)
        {
            var schemaType = GetSchemaTypeFrom(value.GetType());
            if (!_schema.ActualProperties[property].Type.HasFlag(schemaType))
            {
                throw new InvalidTypeForProperty(_schema.Title, property);
            }
        }
    }

    JsonObjectType GetSchemaTypeFrom(Type type)
    {
        return type switch
        {
            Type _ when type == typeof(string) => JsonObjectType.String,
            Type _ when type == typeof(DateOnly) => JsonObjectType.String,
            Type _ when type == typeof(int) => JsonObjectType.Integer,
            Type _ when type == typeof(float) => JsonObjectType.Number,
            Type _ when type == typeof(double) => JsonObjectType.Number,
            _ => JsonObjectType.Object
        };
    }
}
