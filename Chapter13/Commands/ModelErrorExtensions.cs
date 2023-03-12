using Fundamentals;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chapter13.Commands;

public static class ModelErrorExtensions
{
    public static ValidationResult ToValidationResult(this ModelError error, string member)
    {
        member = string.Join('.', member.Split('.').Select(_ => _.ToCamelCase()));
        return new ValidationResult(error.ErrorMessage, member);
    }
}
