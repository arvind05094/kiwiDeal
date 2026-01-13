using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Users.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(UsersDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant().Trim(), cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == token), cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        context.Users.Update(user);
    }

    public async Task<UserRating?> GetRatingAsync(UserId raterId, UserId rateeId, CancellationToken cancellationToken = default)
    {
        return await context.UserRatings
            .FirstOrDefaultAsync(r => r.RaterId == raterId && r.RateeId == rateeId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserRating>> GetRatingsByRateeAsync(UserId rateeId, CancellationToken cancellationToken = default)
    {
        return await context.UserRatings
            .Where(r => r.RateeId == rateeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRatingAsync(UserRating rating, CancellationToken cancellationToken = default)
    {
        await context.UserRatings.AddAsync(rating, cancellationToken);
    }
}
