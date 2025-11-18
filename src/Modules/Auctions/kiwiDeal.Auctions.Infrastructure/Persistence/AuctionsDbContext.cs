using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Auctions.Infrastructure.Persistence;

public sealed class AuctionsDbContext(DbContextOptions<AuctionsDbContext> options)
    : DbContext(options), IAuctionsUnitOfWork
{
    public DbSet<Auction> Auctions => Set<Auction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuctionsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
