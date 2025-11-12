using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using kiwiDeal.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;

namespace kiwiDeal.Api.Infrastructure;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    public Guid? Id
    {
        get
        {
            var value = _httpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? _httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => _httpContext?.User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? _httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => _httpContext?.User?.Identity?.IsAuthenticated ?? false;
}
