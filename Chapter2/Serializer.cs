using System.Text;

namespace Chapter2;

public static class Serializer
{
    public static string SerializeToJson(object instance)
    {
        var stringBuilder = new StringBuilder();
        var type = instance.GetType();
        var properties = type.GetProperties();
        stringBuilder.Append("{\n");
        var first = true;

        foreach (var property in properties)
        {
            if (!first)
            {
                stringBuilder.Append(",\n");
            }
            first = false;
            stringBuilder.Append($"   \"{property.Name}\": \"{property.GetValue(instance)}\"");
        }

        stringBuilder.Append("\n}");

        return stringBuilder.ToString();
    }
}