namespace Chapter14;

public interface IUser
{
    Guid Id { get; set; }
    string UserName { get; set; }
    string Password { get; set; }
}
