using Microsoft.AspNetCore.Authorization;

namespace Chapter13;

public class CrossCuttingPoliciesProvider : IAuthorizationPolicyProvider
{
    readonly AuthorizationPolicy _policy;

    public CrossCuttingPoliciesProvider()
    {
        _policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new AdminForNamespace("Chapter13")
            ).Build();
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(_policy);
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult<AuthorizationPolicy?>(_policy);
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) => Task.FromResult<AuthorizationPolicy?>(_policy);
}
