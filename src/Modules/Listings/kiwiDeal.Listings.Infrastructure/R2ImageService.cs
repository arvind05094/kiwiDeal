using Supabase;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace kiwiDeal.Listings.Infrastructure;

public sealed class R2ImageService : IImageService
{
    private readonly Supabase.Client _supabaseClient;
    private readonly string _bucketName;
    private const int MaxFileSizeBytes = 8 * 1024 * 1024;
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
        var url = configuration["Supabase:Url"]!;
        var key = configuration["Supabase:Key"]!;
        _bucketName = configuration["Supabase:BucketName"]!;

        var options = new SupabaseOptions { AutoConnectRealtime = false };
        _supabaseClient = new Supabase.Client(url, key, options);

        // Block-initialize the client synchonously since this is a constructor injection block
        _supabaseClient.InitializeAsync().Wait();
    }

    public async Task<Result<string>> UploadImageAsync(
        Guid listingId,
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (imageStream.Length > MaxFileSizeBytes)
            return Result.Failure<string>(Error.ValidationFailed("File size exceeds the maximum allowed size of 8MB."));

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

        // Upload to Supabase Storage Bucket
        await _supabaseClient.Storage
            .From(_bucketName)
            .Upload(outputStream.ToArray(), key);

        // Retrieve native public address
        string publicUrl = _supabaseClient.Storage
            .From(_bucketName)
            .GetPublicUrl(key);

        return Result.Success(publicUrl);
    }

    public async Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        // Extracts file path pattern out of a standard url string
        var uri = new Uri(imageUrl);
        var segments = uri.AbsolutePath.Split(new[] { $"/{_bucketName}/" }, StringSplitOptions.None);

        if (segments.Length > 1)
        {
            var key = segments[1];
            await _supabaseClient.Storage.From(_bucketName).Remove(new List<string> { key });
        }
    }
}
