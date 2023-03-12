using Microsoft.AspNetCore.Mvc;

namespace Chapter13;

[Route("/api/employees")]
public class EmployeesController : Controller
{
    [HttpPost]
    public int Register([FromBody] Employee employee)
    {
        return 1;
    }
}
