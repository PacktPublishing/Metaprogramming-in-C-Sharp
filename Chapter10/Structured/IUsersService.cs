namespace Chapter10.Structured;

public interface IUsersService
{
    Task<Guid> Register(string userName, string password);
}
