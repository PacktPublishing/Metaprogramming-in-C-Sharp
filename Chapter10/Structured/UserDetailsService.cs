namespace Chapter10.Structured;

public class UserDetailsService : IUserDetailsService
{
    readonly IDatabase _database;

    public UserDetailsService(IDatabase database)
    {
        _database = database;
    }

    public Task Register(string firstName, string lastName, string socialSecurityNumber, Guid userId)
        => _database.GetCollectionFor<UserDetails>().InsertOneAsync(new(Guid.NewGuid(), userId, firstName, lastName, socialSecurityNumber));
}
