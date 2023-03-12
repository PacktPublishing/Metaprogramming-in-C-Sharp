using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Chapter13;

public class HardCodedAuthenticationHandler : AuthenticationHandler<HardCodedAuthenticationOptions>
{
    public const string SchemeName = "HardCodedAuthenticationHandler";

    public HardCodedAuthenticationHandler(
        IOptionsMonitor<HardCodedAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(
        AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.Name, "Bob"),
                            new Claim(ClaimTypes.Role, "User")
                        },
                        SchemeName)), SchemeName)));
}
