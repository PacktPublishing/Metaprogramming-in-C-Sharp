using Microsoft.AspNetCore.Authorization;

namespace Chapter13;

public class AdminForNamespace : IAuthorizationRequirement
{
    public AdminForNamespace(string @namespace)
    {
        Namespace = @namespace;
    }

    public string Namespace { get; }
}