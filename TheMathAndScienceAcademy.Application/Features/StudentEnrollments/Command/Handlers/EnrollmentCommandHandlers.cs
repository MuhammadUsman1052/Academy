using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Command.Models;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Command.Handlers;

public class EnrollmentCommandHandlers : ResponseHandler, IRequestHandler<CreateStudentEnrollmentCommand, ApiResponse<StudentEnrollmentDto>>, IRequestHandler<UpdateStudentEnrollmentCommand, ApiResponse<StudentEnrollmentDto>>, IRequestHandler<DeleteStudentEnrollmentCommand, ApiResponse<bool>>
{
    private readonly IStudentEnrollmentRepository _enrollments; private readonly IStudentRepository _students; private readonly IAcademicClassRepository _classes; private readonly IAcademyScopeService _scope; private readonly IMapper _mapper;
    public EnrollmentCommandHandlers(IStudentEnrollmentRepository enrollments, IStudentRepository students, IAcademicClassRepository classes, IAcademyScopeService scope, IMapper mapper) { _enrollments = enrollments; _students = students; _classes = classes; _scope = scope; _mapper = mapper; }
    public async Task<ApiResponse<StudentEnrollmentDto>> Handle(CreateStudentEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync(); if (!scope.IsValid || string.IsNullOrWhiteSpace(scope.AcademyId)) return BadRequest<StudentEnrollmentDto>(ResponseMessages.AcademySelectionRequired);
        var student = await _students.GetByIdAsync(request.StudentId.ToString()); var itemClass = await _classes.GetByIdAsync(request.ClassId.ToString());
        if (student is null) return NotFound<StudentEnrollmentDto>(ResponseMessages.StudentNotFound); if (itemClass is null) return NotFound<StudentEnrollmentDto>(ResponseMessages.ClassNotFound);
        if (student.AcademyId != scope.AcademyId || itemClass.AcademyId != scope.AcademyId) return BadRequest<StudentEnrollmentDto>(ResponseMessages.Forbidden);
        if (await _enrollments.GetActiveDuplicateAsync(student.Id, request.AcademicYear.Trim(), scope.AcademyId) is not null) return BadRequest<StudentEnrollmentDto>(ResponseMessages.StudentAlreadyEnrolled);
        var enrollment = new StudentEnrollment { StudentId = student.Id, ClassId = itemClass.Id, AcademyId = scope.AcademyId, AcademicYear = request.AcademicYear.Trim(), EnrollmentDate = DateTime.SpecifyKind(request.EnrollmentDate.Date, DateTimeKind.Utc), Status = "Active" };
        var created = await _enrollments.CreateAsync(enrollment); return created is null ? BadRequest<StudentEnrollmentDto>(ResponseMessages.EnrollmentCreateFailed) : Created(await ToDtoAsync(created, student.FullName, itemClass.Name), ResponseMessages.EnrollmentCreated);
    }
    public async Task<ApiResponse<StudentEnrollmentDto>> Handle(UpdateStudentEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync(); var enrollment = await _enrollments.GetByIdAsync(request.Id.ToString()); if (enrollment is null) return NotFound<StudentEnrollmentDto>(ResponseMessages.EnrollmentNotFound); if (enrollment.AcademyId != scope.AcademyId) return BadRequest<StudentEnrollmentDto>(ResponseMessages.Forbidden);
        enrollment.EndDate = request.EndDate.HasValue ? DateTime.SpecifyKind(request.EndDate.Value.Date, DateTimeKind.Utc) : null; enrollment.Status = request.Status; enrollment.UpdatedAt = DateTime.UtcNow;
        if (!await _enrollments.UpdateAsync(enrollment)) return BadRequest<StudentEnrollmentDto>(ResponseMessages.EnrollmentUpdateFailed);
        return Updated(await ToDtoAsync(enrollment), ResponseMessages.EnrollmentUpdated);
    }
    public async Task<ApiResponse<bool>> Handle(DeleteStudentEnrollmentCommand request, CancellationToken cancellationToken)
    { var scope = await _scope.GetScopeAsync(); var enrollment = await _enrollments.GetByIdAsync(request.Id.ToString()); if (enrollment is null) return NotFound<bool>(ResponseMessages.EnrollmentNotFound); if (enrollment.AcademyId != scope.AcademyId) return BadRequest<bool>(ResponseMessages.Forbidden); if (!await _enrollments.DeleteAsync(enrollment.Id)) return NotFound<bool>(ResponseMessages.EnrollmentNotFound); return Deleted<bool>(ResponseMessages.EnrollmentDeleted); }
    private async Task<StudentEnrollmentDto> ToDtoAsync(StudentEnrollment item, string? studentName = null, string? className = null) { var student = studentName is null ? await _students.GetByIdAsync(item.StudentId) : null; var itemClass = className is null ? await _classes.GetByIdAsync(item.ClassId) : null; var dto = _mapper.Map<StudentEnrollmentDto>(item); dto.StudentName = studentName ?? student?.FullName; dto.ClassName = className ?? itemClass?.Name; return dto; }
}
