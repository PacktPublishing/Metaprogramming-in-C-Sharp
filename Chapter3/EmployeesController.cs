using Microsoft.AspNetCore.Mvc;

namespace Chapter3;

[Route("/api/employees")]
public class EmployeesController : Controller
{
    [HttpPost]
    public IActionResult Register(Employee employee)
    {
        return Ok();
    }

    [HttpGet]
    public IEnumerable<Employee> AllEmployees()
    {
        return new Employee[]
        {
            new("Jane", "Doe"),
            new("John", "Doe")
        };
    }
}
