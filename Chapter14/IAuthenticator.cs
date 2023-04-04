namespace Chapter14;

public interface IAuthenticator
{
    bool Authenticate(string username, string password);
}
