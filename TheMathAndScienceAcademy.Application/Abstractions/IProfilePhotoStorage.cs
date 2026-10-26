namespace TheMathAndScienceAcademy.Application.Abstractions;

public sealed record ProfilePhotoUpload(Stream Content, string ContentType, long Length);
public sealed record StoredProfilePhoto(string Key, string ContentType, DateTime UpdatedAt);
public sealed record ProfilePhotoContent(Stream Content, string ContentType);

public interface IProfilePhotoStorage
{
    Task<StoredProfilePhoto> SaveAsync(string academyId, string profileType, string profileId, ProfilePhotoUpload upload, CancellationToken cancellationToken = default);
    Task<ProfilePhotoContent?> OpenReadAsync(string key, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string? key, CancellationToken cancellationToken = default);
}
