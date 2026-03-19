using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace kiwiDeal.Listings.Infrastructure;

public sealed class R2ImageService : IImageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly string _publicUrl;
    private const int MaxFileSizeBytes = 1 * 1024 * 1024;
    private const int MaxWidthPx = 1200;
    private const int JpegQuality = 80;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/jpg", "image/png", "image/webp"
        };

    public R2ImageService(IConfiguration configuration)
    {
        var accountId = configuration["R2:AccountId"]!;
        var accessKey = configuration["R2:AccessKey"]!;
        var secretKey = configuration["R2:SecretKey"]!;
        _bucketName = configuration["R2:BucketName"]!;
        _publicUrl = configuration["R2:PublicUrl"]!.TrimEnd('/');

        var config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), config);
    }

    public async Task<Result<string>> UploadImageAsync(
        Guid listingId,
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (imageStream.Length > MaxFileSizeBytes)
            return Result.Failure<string>(Error.ValidationFailed("File size exceeds the maximum allowed size of 1MB."));

        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension))
            return Result.Failure<string>(Error.ValidationFailed($"File type '{extension}' is not allowed. Allowed types: jpg, jpeg, png, webp."));

        if (!AllowedContentTypes.Contains(contentType))
            return Result.Failure<string>(Error.ValidationFailed($"Content type '{contentType}' is not allowed."));

        using var image = await Image.LoadAsync(imageStream, cancellationToken);

        if (image.Width > MaxWidthPx)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(MaxWidthPx, 0),
                Mode = ResizeMode.Max
            }));
        }

        var encoder = new JpegEncoder { Quality = JpegQuality };
        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, encoder, cancellationToken);
        outputStream.Position = 0;

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var key = $"listings/{listingId}/{timestamp}.jpg";

        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = outputStream,
            ContentType = "image/jpeg"
        }, cancellationToken);

        return Result.Success($"{_publicUrl}/{key}");
    }

    public async Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var key = imageUrl[(imageUrl.IndexOf(_publicUrl, StringComparison.Ordinal) + _publicUrl.Length + 1)..];
        await _s3Client.DeleteObjectAsync(_bucketName, key, cancellationToken);
    }
}
