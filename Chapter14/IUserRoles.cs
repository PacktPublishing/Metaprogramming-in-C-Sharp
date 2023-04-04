namespace Chapter14;

public interface IUserRoles
{
    Guid Id { get; set; }
    IEnumerable<string> Roles { get; set; }
}
