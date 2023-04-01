namespace Chapter14;

public interface IUsersService
{
    Task<Guid> Register(string userName, string password);
}
