using Concepts.Employees;
using Fundamentals;

namespace Domain.Employees;

public class RegisterEmployee : ICommand
{
    public FirstName FirstName { get; set; } = new(string.Empty);
    public LastName LastName { get; set; } = new(string.Empty);
    public SocialSecurityNumber SocialSecurityNumber { get; set; } = new(string.Empty);
}
