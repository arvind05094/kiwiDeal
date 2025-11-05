namespace kiwiDeal.Listings.Domain.Repositories;

public interface IImageService
{
    Task<string> UploadImageAsync(Guid listingId, Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
