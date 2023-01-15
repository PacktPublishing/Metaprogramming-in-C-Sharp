using MongoDB.Driver;

namespace Chapter10.Structured;

public interface IDatabase
{
    IMongoCollection<T> GetCollectionFor<T>();
}
