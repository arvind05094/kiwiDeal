using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Listings.Domain.Repositories;

public interface IImageService
{
    Task<Result<string>> UploadImageAsync(Guid listingId, Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
