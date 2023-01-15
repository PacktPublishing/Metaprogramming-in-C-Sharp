namespace Chapter10.Structured;

public class UsersService : IUsersService
{
    readonly IDatabase _database;

    public UsersService(IDatabase database)
    {
        _database = database;
    }

    public async Task<Guid> Register(string userName, string password)
    {
        var user = new User(Guid.NewGuid(), userName, password);
        await _database.GetCollectionFor<User>().InsertOneAsync(user);
        return user.Id;
    }
}
