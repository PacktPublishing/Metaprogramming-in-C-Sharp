using Microsoft.AspNetCore.Mvc;

namespace Chapter10.Structured;

[Route("/api/users")]
public class UsersController : Controller
{
    readonly IUsersService _usersService;
    readonly IUserDetailsService _userDetailsService;

    public UsersController(IUsersService usersService, IUserDetailsService userDetailsService)
    {
        _usersService = usersService;
        _userDetailsService = userDetailsService;
    }

    [HttpPost("register")]
    public async Task Register([FromBody] RegisterUser userRegistration)
    {
        var userId = await _usersService.Register(userRegistration.UserName, userRegistration.Password);
        await _userDetailsService.Register(
            userRegistration.FirstName,
            userRegistration.LastName,
            userRegistration.SocialSecurityNumber,
            userId);
    }
}