
using Microsoft.Extensions.Logging;

namespace Chapter14;

public class UsersService : IUsersService
{
    readonly ILogger<UsersService> _logger;

    public UsersService(ILogger<UsersService> logger)
    {
        _logger = logger;
    }

    public Task<Guid> Register(string userName, string password)
    {
        _logger.LogInformation("Inside register method");
        var id = Guid.NewGuid();
        return Task.FromResult(id);
    }
}
