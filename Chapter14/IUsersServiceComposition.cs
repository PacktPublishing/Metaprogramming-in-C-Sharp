namespace Chapter14;

public interface IUsersServiceComposition : IUsersService, IAuthenticator, IAuthorizer
{
}
