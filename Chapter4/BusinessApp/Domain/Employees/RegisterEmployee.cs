using Fundamentals;

namespace Domain.Employees;

public class RegisterEmployee : ICommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SocialSecurityNumber { get; set; } = string.Empty;
}
