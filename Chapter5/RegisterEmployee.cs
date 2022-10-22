using Fundamentals;

namespace Main;

public class RegisterEmployee
{
    [PersonalIdentifiableInformation("Employment records")]
    public string FirstName { get; set; } = string.Empty;

    [PersonalIdentifiableInformation("Employment records")]
    public string LastName { get; set; } = string.Empty;

    [PersonalIdentifiableInformation("Uniquely identifies the employee")]
    public string SocialSecurityNumber { get; set; } = string.Empty;
}
