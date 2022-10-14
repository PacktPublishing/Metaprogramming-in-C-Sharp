using Fundamentals;

namespace Concepts.Employees;

public record SocialSecurityNumber(string Value) : PIIConceptAs<string>(Value);