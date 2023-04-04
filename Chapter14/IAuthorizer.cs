namespace Chapter14;

public interface IAuthorizer
{
    bool IsAuthorized(string username, string action);
}
