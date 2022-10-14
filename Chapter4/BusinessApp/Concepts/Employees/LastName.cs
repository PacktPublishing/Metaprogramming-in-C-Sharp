using Fundamentals;

namespace Concepts.Employees;

public record LastName(string Value) : PIIConceptAs<string>(Value);
