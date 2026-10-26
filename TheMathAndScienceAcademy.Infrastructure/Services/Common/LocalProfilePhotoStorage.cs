using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Configuration;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Infrastructure.Services.Common;

public sealed class LocalProfilePhotoStorage : IProfilePhotoStorage
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private readonly string _root;

    public LocalProfilePhotoStorage(IConfiguration configuration)
    {
        var configuredPath = configuration["ProfilePhotoStorage:RootPath"];
        _root = Path.GetFullPath(Path.IsPathFullyQualified(configuredPath)
            ? configuredPath
            : Path.Combine(AppContext.BaseDirectory, configuredPath ?? "App_Data/profile-photos"));
    }

    public async Task<StoredProfilePhoto> SaveAsync(string academyId, string profileType, string profileId, ProfilePhotoUpload upload, CancellationToken cancellationToken = default)
    {
        if (upload.Length <= 0 || upload.Length > MaxBytes || !IsAllowedContentType(upload.ContentType))
            throw new InvalidOperationException("Upload a JPEG, PNG, or WebP image no larger than 5 MB.");

        try
        {
            var format = await Image.DetectFormatAsync(upload.Content, cancellationToken);
            if (format is null || !IsAllowedFormat(format.Name))
                throw new InvalidOperationException("Upload a JPEG, PNG, or WebP image no larger than 5 MB.");
            if (upload.Content.CanSeek) upload.Content.Position = 0;
            using var image = await Image.LoadAsync(upload.Content, cancellationToken);
            image.Metadata.ExifProfile = null;
            image.Metadata.IptcProfile = null;
            image.Metadata.XmpProfile = null;
            image.Mutate(context => context.AutoOrient().Resize(new ResizeOptions { Size = new Size(512, 512), Mode = ResizeMode.Crop, Position = AnchorPositionMode.Center }));

            var key = Guid.NewGuid().ToString("N");
            var directory = Path.Combine(_root, academyId, profileType, profileId);
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, key + ".jpg");
            await image.SaveAsJpegAsync(path, new JpegEncoder { Quality = 85 }, cancellationToken);
            return new StoredProfilePhoto(BuildKey(academyId, profileType, profileId, key), "image/jpeg", DateTime.UtcNow);
        }
        catch (InvalidOperationException) { throw; }
        catch { throw new InvalidOperationException("Upload a valid JPEG, PNG, or WebP image no larger than 5 MB."); }
    }

    public Task<ProfilePhotoContent?> OpenReadAsync(string key, string contentType, CancellationToken cancellationToken = default)
    {
        var path = GetPath(key);
        if (path is null || !File.Exists(path)) return Task.FromResult<ProfilePhotoContent?>(null);
        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
        return Task.FromResult<ProfilePhotoContent?>(new ProfilePhotoContent(stream, contentType));
    }

    public Task DeleteAsync(string? key, CancellationToken cancellationToken = default)
    {
        var path = GetPath(key);
        if (path is not null && File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private string? GetPath(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        var parts = key.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4 || !Guid.TryParse(parts[0], out _) || !Guid.TryParse(parts[2], out _) || !Guid.TryParseExact(parts[3], "N", out _) || (parts[1] != "students" && parts[1] != "teachers")) return null;
        return Path.Combine(_root, parts[0], parts[1], parts[2], parts[3] + ".jpg");
    }

    private static string BuildKey(string academyId, string profileType, string profileId, string fileKey) => string.Join('/', academyId, profileType, profileId, fileKey);
    private static bool IsAllowedContentType(string value) => value.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) || value.Equals("image/png", StringComparison.OrdinalIgnoreCase) || value.Equals("image/webp", StringComparison.OrdinalIgnoreCase);
    private static bool IsAllowedFormat(string value) => value.Equals("JPEG", StringComparison.OrdinalIgnoreCase) || value.Equals("PNG", StringComparison.OrdinalIgnoreCase) || value.Equals("WEBP", StringComparison.OrdinalIgnoreCase);
}
