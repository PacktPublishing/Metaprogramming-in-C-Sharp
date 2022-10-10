namespace Fundamentals;

/// <summary>
/// Defines information for types.
/// </summary>
public interface ITypeInfo
{
    /// <summary>
    /// Gets a value indicating whether or not the type has a default constructor that takes no arguments.
    /// </summary>
    bool HasDefaultConstructor { get; }
}
