using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Chapter10.Unstructured;

public record RegisterUser(string FirstName, string LastName, string SocialSecurityNumber, string UserName, string Password);

public record User(Guid Id, string UserName, string Password);

public record UserDetails(Guid Id, Guid UserId, string FirstName, string LastName, string SocialSecurityNumber);

[Route("/api/unstructured/users")]
public class UsersController : Controller
{
    IMongoCollection<User> _userCollection;
    IMongoCollection<UserDetails> _userDetailsCollection;

    public UsersController()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("TheSystem");
        _userCollection = database.GetCollection<User>("users");
        _userDetailsCollection = database.GetCollection<UserDetails>("user-details");
    }

    [HttpPost("register")]
    public async Task Register([FromBody] RegisterUser userRegistration)
    {
        var user = new User(Guid.NewGuid(), userRegistration.UserName, userRegistration.Password);
        var userDetails = new UserDetails(Guid.NewGuid(), user.Id, userRegistration.FirstName, userRegistration.LastName, userRegistration.SocialSecurityNumber);
        await _userCollection.InsertOneAsync(user);
        await _userDetailsCollection.InsertOneAsync(userDetails);
    }
}