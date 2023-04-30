namespace Fundamentals.Metrics;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CounterAttribute<T> : Attribute
{
    public CounterAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string Description { get; }
}


