namespace Fundamentals;

/// <summary>
/// Represents a <see cref="ConceptAs{T}"/> that holds PII according to the definition of GDPR.
/// </summary>
/// <param name="Value">Underlying value.</param>
/// <typeparam name="T">Type of the underlying value.</typeparam>
public record PIIConceptAs<T>(T Value) : ConceptAs<T>(Value)
{
    /// <summary>
    /// Gets the reason for collecting the PII.
    /// </summary>
    public virtual string ReasonForCollecting => string.Empty;
}
