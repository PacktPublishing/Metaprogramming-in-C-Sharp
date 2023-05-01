using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Chapter16;

[Route("/api/employees")]
public class EmployeesController : Controller
{
    static Counter<int> _registeredEmployees = Metrics.Meter.CreateCounter<int>("RegisteredEmployees", "# of registered employees");

    [HttpGet("manual")]
    public IActionResult RegisterManual()
    {
        var now = DateTimeOffset.UtcNow;
        var tags = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(new KeyValuePair<string, object?>[]
        {
            new("Year", now.Year),
            new("Month", now.Month),
            new("Day", now.Day),
        }));

        _registeredEmployees.Add(1, tags);

        return Ok();
    }

    [HttpGet]
    public IActionResult Register()
    {
        EmployeesControllerMetrics.RegisteredEmployees(DateOnly.FromDateTime(DateTime.UtcNow));

        return Ok();
    }
}
