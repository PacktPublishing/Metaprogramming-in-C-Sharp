using Fundamentals;

namespace Domain.Employees;

public class SetSalaryLevelForEmployee : ICommand
{
    public string SocialSecurityNumber { get; set; } = string.Empty;
    public decimal SalaryLevel { get; set; }
}