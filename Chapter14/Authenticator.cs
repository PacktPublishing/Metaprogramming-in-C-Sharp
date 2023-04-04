namespace Chapter14;

public class Authenticator : IAuthenticator
{
    public bool Authenticate(string username, string password)
    {
        return true;
    }
}
