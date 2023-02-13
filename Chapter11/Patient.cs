using Fundamentals.Compliance.GDPR;

namespace Chapter11;

[Confidential]
public class Patient
{
    [PersonalIdentifiableInformation("Employment records")]
    public string FirstName { get; set; } = string.Empty;

    [PersonalIdentifiableInformation("Employment records")]
    public string LastName { get; set; } = string.Empty;

    [PersonalIdentifiableInformation("Uniquely identifies the employee")]
    public string SocialSecurityNumber { get; set; } = string.Empty;

    public IEnumerable<JournalEntry> JournalEntries { get; set; } = Enumerable.Empty<JournalEntry>();
}
