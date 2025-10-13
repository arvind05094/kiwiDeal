using kiwiDeal.Users.Domain.Entities;

namespace kiwiDeal.Users.Domain.Repositories;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
