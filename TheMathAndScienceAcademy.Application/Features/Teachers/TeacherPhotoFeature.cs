using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Teachers;

public record UploadTeacherPhotoCommand(Guid TeacherId, ProfilePhotoUpload Upload) : IRequest<ApiResponse<bool>>;
public record DeleteTeacherPhotoCommand(Guid TeacherId) : IRequest<ApiResponse<bool>>;
public record GetTeacherPhotoQuery(Guid TeacherId) : IRequest<ApiResponse<ProfilePhotoContent>>;
public record GetMyTeacherPhotoQuery : IRequest<ApiResponse<ProfilePhotoContent>>;

public class TeacherPhotoHandlers : ResponseHandler,
    IRequestHandler<UploadTeacherPhotoCommand, ApiResponse<bool>>, IRequestHandler<DeleteTeacherPhotoCommand, ApiResponse<bool>>,
    IRequestHandler<GetTeacherPhotoQuery, ApiResponse<ProfilePhotoContent>>, IRequestHandler<GetMyTeacherPhotoQuery, ApiResponse<ProfilePhotoContent>>
{
    private readonly ITeacherRepository _teachers; private readonly IAcademyScopeService _scope; private readonly ICurrentUserService _current; private readonly IProfilePhotoStorage _photos;
    public TeacherPhotoHandlers(ITeacherRepository teachers, IAcademyScopeService scope, ICurrentUserService current, IProfilePhotoStorage photos) { _teachers = teachers; _scope = scope; _current = current; _photos = photos; }
    public async Task<ApiResponse<bool>> Handle(UploadTeacherPhotoCommand request, CancellationToken ct) { var teacher = await _teachers.GetByIdAsync(request.TeacherId.ToString()); if (teacher is null) return NotFound<bool>(ResponseMessages.TeacherNotFound); if (!await CanAccess(teacher.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden); try { var saved = await _photos.SaveAsync(teacher.AcademyId, "teachers", teacher.Id, request.Upload, ct); var oldKey = teacher.ProfilePhotoKey; teacher.ProfilePhotoKey = saved.Key; teacher.ProfilePhotoContentType = saved.ContentType; teacher.ProfilePhotoUpdatedAt = saved.UpdatedAt; teacher.UpdatedAt = DateTime.UtcNow; if (!await _teachers.UpdateAsync(teacher)) { await _photos.DeleteAsync(saved.Key, ct); return BadRequest<bool>(ResponseMessages.TeacherNotFound); } await _photos.DeleteAsync(oldKey, ct); return Success(true, ResponseMessages.PhotoUpdated); } catch (InvalidOperationException ex) { return BadRequest<bool>(ex.Message); } }
    public async Task<ApiResponse<bool>> Handle(DeleteTeacherPhotoCommand request, CancellationToken ct) { var teacher = await _teachers.GetByIdAsync(request.TeacherId.ToString()); if (teacher is null) return NotFound<bool>(ResponseMessages.TeacherNotFound); if (!await CanAccess(teacher.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden); var oldKey = teacher.ProfilePhotoKey; teacher.ProfilePhotoKey = null; teacher.ProfilePhotoContentType = null; teacher.ProfilePhotoUpdatedAt = null; teacher.UpdatedAt = DateTime.UtcNow; await _teachers.UpdateAsync(teacher); await _photos.DeleteAsync(oldKey, ct); return Success(true, ResponseMessages.PhotoDeleted); }
    public async Task<ApiResponse<ProfilePhotoContent>> Handle(GetTeacherPhotoQuery request, CancellationToken ct) { var teacher = await _teachers.GetByIdAsync(request.TeacherId.ToString()); if (teacher is null) return NotFound<ProfilePhotoContent>(ResponseMessages.TeacherNotFound); if (!await CanAccess(teacher.AcademyId)) return BadRequest<ProfilePhotoContent>(ResponseMessages.Forbidden); return await Read(teacher.ProfilePhotoKey, teacher.ProfilePhotoContentType, ct); }
    public async Task<ApiResponse<ProfilePhotoContent>> Handle(GetMyTeacherPhotoQuery request, CancellationToken ct) { if (string.IsNullOrWhiteSpace(_current.UserId)) return BadRequest<ProfilePhotoContent>(ResponseMessages.Unauthorized); var teacher = await _teachers.GetByUserIdAsync(_current.UserId); if (teacher is null || !await CanAccess(teacher.AcademyId)) return NotFound<ProfilePhotoContent>(ResponseMessages.PhotoNotFound); return await Read(teacher.ProfilePhotoKey, teacher.ProfilePhotoContentType, ct); }
    private async Task<ApiResponse<ProfilePhotoContent>> Read(string? key, string? contentType, CancellationToken ct) { if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(contentType)) return NotFound<ProfilePhotoContent>(ResponseMessages.PhotoNotFound); var photo = await _photos.OpenReadAsync(key, contentType, ct); return photo is null ? NotFound<ProfilePhotoContent>(ResponseMessages.PhotoNotFound) : Success(photo); }
    private async Task<bool> CanAccess(string academyId) { var scope = await _scope.GetScopeAsync(); return scope.IsValid && scope.AcademyId == academyId; }
}
