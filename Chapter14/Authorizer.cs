namespace Chapter14;

public class Authorizer : IAuthorizer
{
    public bool IsAuthorized(string username, string action)
    {
        return false;
    }
}
