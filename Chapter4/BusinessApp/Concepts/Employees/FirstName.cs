using Fundamentals;

namespace Concepts.Employees;

public record FirstName(string Value) : PIIConceptAs<string>(Value);
