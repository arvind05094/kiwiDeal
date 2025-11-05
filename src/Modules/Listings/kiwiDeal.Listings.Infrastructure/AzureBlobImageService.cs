using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using kiwiDeal.Listings.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace kiwiDeal.Listings.Infrastructure;

public sealed class AzureBlobImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "listing-images";

    public AzureBlobImageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage")!;
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(
        Guid listingId,
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blobName = $"{listingId}/{Guid.CreateVersion7()}-{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(imageStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(imageUrl);
        var blobName = string.Join("/", uri.Segments[2..]);
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
