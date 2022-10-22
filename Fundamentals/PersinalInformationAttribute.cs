namespace Fundamentals;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class PersonalIdentifiableInformationAttribute : Attribute
{
    public PersonalIdentifiableInformationAttribute(string reasonForCollecting = "")
    {
        ReasonForCollecting = reasonForCollecting;
    }

    public string ReasonForCollecting { get; }
}
