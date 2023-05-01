using Fundamentals.Compliance.GDPR;

namespace Chapter16;

public record Employee(
    [PersonalIdentifiableInformation("Needed for registration")]
    string FirstName,

    [PersonalIdentifiableInformation("Needed for registration")]
    string LastName,

    [PersonalIdentifiableInformation("Needed for uniquely identifying an employee")]
    string SocialSecurityNumber);