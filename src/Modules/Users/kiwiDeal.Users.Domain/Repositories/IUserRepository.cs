using kiwiDeal.Users.Domain.Entities;

namespace kiwiDeal.Users.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
    Task<UserRating?> GetRatingAsync(UserId raterId, UserId rateeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRating>> GetRatingsByRateeAsync(UserId rateeId, CancellationToken cancellationToken = default);
    Task AddRatingAsync(UserRating rating, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<UserRating> Items, int TotalCount)> GetPagedRatingsByRateeAsync(
    UserId rateeId, int skip, int take, CancellationToken cancellationToken = default);
}
