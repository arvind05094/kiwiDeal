using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace kiwiDeal.Tests.Integration.Listings;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static Guid SellerId { get; } = Guid.NewGuid();
    public static Guid BidderId { get; } = Guid.NewGuid();

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue("X-Integration-Bidder", out _))
        {
            var bidderClaims = new[]
            {
                new Claim("sub", BidderId.ToString()),
                new Claim(ClaimTypes.Role, "Bidder")
            };
            var bidderIdentity = new ClaimsIdentity(bidderClaims, "Test");
            var bidderPrincipal = new ClaimsPrincipal(bidderIdentity);
            var bidderTicket = new AuthenticationTicket(bidderPrincipal, "Test");
            return Task.FromResult(AuthenticateResult.Success(bidderTicket));
        }

        if (!Request.Headers.ContainsKey("X-Integration-Test"))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new[]
        {
            new Claim("sub", SellerId.ToString()),
            new Claim(ClaimTypes.Role, "Seller")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
