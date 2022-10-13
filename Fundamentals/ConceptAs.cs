namespace Fundamentals;

public record ConceptAs<T>
{
    public ConceptAs(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        Value = value;
    }

    public T Value { get; init; }

    public static implicit operator T(ConceptAs<T> value) => value.Value;

    public sealed override string ToString()
    {
        return Value!.ToString() ?? "[n/a]";
    }
}
