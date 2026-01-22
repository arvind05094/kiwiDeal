using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace kiwiDeal.Listings.Infrastructure;

public sealed class AzureBlobImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "kiwideal-images";
    private const int MaxFileSizeBytes = 1 * 1024 * 1024; // 1MB
    private const int MaxWidthPx = 1200;
    private const int JpegQuality = 80;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/jpg", "image/png", "image/webp"
        };

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
        // Validate file size
        if (imageStream.Length > MaxFileSizeBytes)
            throw new InvalidOperationException($"File size exceeds the maximum allowed size of 1MB.");

        // Validate extension
        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: jpg, jpeg, png, webp.");

        // Validate content type
        if (!AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException($"Content type '{contentType}' is not allowed.");

        // Process image with ImageSharp
        using var image = await Image.LoadAsync(imageStream, cancellationToken);

        // Resize if wider than max — never upscale
        if (image.Width > MaxWidthPx)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(MaxWidthPx, 0),
                Mode = ResizeMode.Max
            }));
        }

        // Encode as JPEG with quality 80
        var encoder = new JpegEncoder { Quality = JpegQuality };
        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, encoder, cancellationToken);
        outputStream.Position = 0;

        // Upload to Azure Blob Storage
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var blobName = $"listings/{listingId}/{timestamp}.jpg";

        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(outputStream, new BlobHttpHeaders { ContentType = "image/jpeg" }, cancellationToken: cancellationToken);

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
