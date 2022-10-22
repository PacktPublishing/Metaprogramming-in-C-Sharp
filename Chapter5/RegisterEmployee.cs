using Fundamentals;

namespace Main;

public class RegisterEmployee
{
    [PersonalIdentifiableInformation]
    public string FirstName { get; set; } = string.Empty;

    [PersonalIdentifiableInformation]
    public string LastName { get; set; } = string.Empty;

    [PersonalIdentifiableInformation]
    public string SocialSecurityNumber { get; set; } = string.Empty;
}
