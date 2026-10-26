using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Students;

public record UploadStudentPhotoCommand(Guid StudentId, ProfilePhotoUpload Upload) : IRequest<ApiResponse<bool>>;
public record DeleteStudentPhotoCommand(Guid StudentId) : IRequest<ApiResponse<bool>>;
public record GetStudentPhotoQuery(Guid StudentId) : IRequest<ApiResponse<ProfilePhotoContent>>;
public record GetMyStudentPhotoQuery : IRequest<ApiResponse<ProfilePhotoContent>>;

public class StudentPhotoHandlers : ResponseHandler,
    IRequestHandler<UploadStudentPhotoCommand, ApiResponse<bool>>, IRequestHandler<DeleteStudentPhotoCommand, ApiResponse<bool>>,
    IRequestHandler<GetStudentPhotoQuery, ApiResponse<ProfilePhotoContent>>, IRequestHandler<GetMyStudentPhotoQuery, ApiResponse<ProfilePhotoContent>>
{
    private readonly IStudentRepository _students; private readonly IAcademyScopeService _scope; private readonly ICurrentUserService _current; private readonly IProfilePhotoStorage _photos;
    public StudentPhotoHandlers(IStudentRepository students, IAcademyScopeService scope, ICurrentUserService current, IProfilePhotoStorage photos) { _students = students; _scope = scope; _current = current; _photos = photos; }
    public async Task<ApiResponse<bool>> Handle(UploadStudentPhotoCommand request, CancellationToken ct) { var student = await _students.GetByIdAsync(request.StudentId.ToString()); if (student is null) return NotFound<bool>(ResponseMessages.StudentNotFound); if (!await CanAccess(student.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden); try { var saved = await _photos.SaveAsync(student.AcademyId, "students", student.Id, request.Upload, ct); var oldKey = student.ProfilePhotoKey; student.ProfilePhotoKey = saved.Key; student.ProfilePhotoContentType = saved.ContentType; student.ProfilePhotoUpdatedAt = saved.UpdatedAt; student.UpdatedAt = DateTime.UtcNow; if (!await _students.UpdateAsync(student)) { await _photos.DeleteAsync(saved.Key, ct); return BadRequest<bool>(ResponseMessages.StudentUpdateFailed); } await _photos.DeleteAsync(oldKey, ct); return Success(true, ResponseMessages.PhotoUpdated); } catch (InvalidOperationException ex) { return BadRequest<bool>(ex.Message); } }
    public async Task<ApiResponse<bool>> Handle(DeleteStudentPhotoCommand request, CancellationToken ct) { var student = await _students.GetByIdAsync(request.StudentId.ToString()); if (student is null) return NotFound<bool>(ResponseMessages.StudentNotFound); if (!await CanAccess(student.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden); var oldKey = student.ProfilePhotoKey; student.ProfilePhotoKey = null; student.ProfilePhotoContentType = null; student.ProfilePhotoUpdatedAt = null; student.UpdatedAt = DateTime.UtcNow; await _students.UpdateAsync(student); await _photos.DeleteAsync(oldKey, ct); return Success(true, ResponseMessages.PhotoDeleted); }
    public async Task<ApiResponse<ProfilePhotoContent>> Handle(GetStudentPhotoQuery request, CancellationToken ct) { var student = await _students.GetByIdAsync(request.StudentId.ToString()); if (student is null) return NotFound<ProfilePhotoContent>(ResponseMessages.StudentNotFound); if (!await CanAccess(student.AcademyId)) return BadRequest<ProfilePhotoContent>(ResponseMessages.Forbidden); return await Read(student.ProfilePhotoKey, student.ProfilePhotoContentType, ct); }
    public async Task<ApiResponse<ProfilePhotoContent>> Handle(GetMyStudentPhotoQuery request, CancellationToken ct) { if (string.IsNullOrWhiteSpace(_current.UserId)) return BadRequest<ProfilePhotoContent>(ResponseMessages.Unauthorized); var student = await _students.GetByUserIdAsync(_current.UserId); if (student is null || !await CanAccess(student.AcademyId)) return NotFound<ProfilePhotoContent>(ResponseMessages.PhotoNotFound); return await Read(student.ProfilePhotoKey, student.ProfilePhotoContentType, ct); }
    private async Task<ApiResponse<ProfilePhotoContent>> Read(string? key, string? contentType, CancellationToken ct) { if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(contentType)) return NotFound<ProfilePhotoContent>(ResponseMessages.PhotoNotFound); var photo = await _photos.OpenReadAsync(key, contentType, ct); return photo is null ? NotFound<ProfilePhotoContent>(ResponseMessages.PhotoNotFound) : Success(photo); }
    private async Task<bool> CanAccess(string academyId) { var scope = await _scope.GetScopeAsync(); return scope.IsValid && scope.AcademyId == academyId; }
}
