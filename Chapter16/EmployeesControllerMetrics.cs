using System.Diagnostics.Metrics;
using Fundamentals.Metrics;

namespace Chapter16;

public static partial class EmployeesControllerMetrics
{
    [Counter<int>("RegisteredEmployees", "# of registered employees")]
    public static partial void RegisteredEmployees(DateOnly date);
}