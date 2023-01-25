using MongoDB.Driver;

namespace Chapter10.Structured;

public class Database : IDatabase
{
    static readonly Dictionary<Type, string> _typeToCollectionName = new()
    {
        { typeof(User), "user" },
        { typeof(UserDetails), "user-details" }
    };
    readonly IMongoDatabase _mongoDatabase;

    public Database()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        _mongoDatabase = client.GetDatabase("TheSystem");
    }

    public IMongoCollection<T> GetCollectionFor<T>() => _mongoDatabase.GetCollection<T>(_typeToCollectionName[typeof(T)]);
}
