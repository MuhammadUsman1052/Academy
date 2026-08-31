using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Dtos;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Query.Models;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Query.Handlers;

public class EnrollmentQueryHandlers : ResponseHandler, IRequestHandler<GetStudentEnrollmentsQuery, ApiResponse<List<StudentEnrollmentDto>>>, IRequestHandler<GetEnrollmentsByStudentQuery, ApiResponse<List<StudentEnrollmentDto>>>, IRequestHandler<GetEnrollmentsByClassQuery, ApiResponse<List<StudentEnrollmentDto>>>
{
    private readonly IStudentEnrollmentRepository _enrollments; private readonly IStudentRepository _students; private readonly IAcademicClassRepository _classes; private readonly IAcademyScopeService _scope; private readonly IMapper _mapper;
    public EnrollmentQueryHandlers(IStudentEnrollmentRepository enrollments, IStudentRepository students, IAcademicClassRepository classes, IAcademyScopeService scope, IMapper mapper) { _enrollments = enrollments; _students = students; _classes = classes; _scope = scope; _mapper = mapper; }
    public async Task<ApiResponse<List<StudentEnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken) { var academy = await ScopeAsync(); if (academy is null) return BadRequest<List<StudentEnrollmentDto>>(ResponseMessages.AcademySelectionRequired); return await ListAsync(await _enrollments.GetByAcademyIdAsync(academy)); }
    public async Task<ApiResponse<List<StudentEnrollmentDto>>> Handle(GetEnrollmentsByStudentQuery request, CancellationToken cancellationToken) { var academy = await ScopeAsync(); if (academy is null) return BadRequest<List<StudentEnrollmentDto>>(ResponseMessages.AcademySelectionRequired); return await ListAsync(await _enrollments.GetByStudentIdAsync(request.StudentId.ToString(), academy)); }
    public async Task<ApiResponse<List<StudentEnrollmentDto>>> Handle(GetEnrollmentsByClassQuery request, CancellationToken cancellationToken) { var academy = await ScopeAsync(); if (academy is null) return BadRequest<List<StudentEnrollmentDto>>(ResponseMessages.AcademySelectionRequired); return await ListAsync(await _enrollments.GetByClassIdAsync(request.ClassId.ToString(), academy)); }
    private async Task<ApiResponse<List<StudentEnrollmentDto>>> ListAsync(List<StudentEnrollment> items) { var result = new List<StudentEnrollmentDto>(); foreach (var item in items) { var dto = _mapper.Map<StudentEnrollmentDto>(item); dto.StudentName = (await _students.GetByIdAsync(item.StudentId))?.FullName; dto.ClassName = (await _classes.GetByIdAsync(item.ClassId))?.Name; result.Add(dto); } return Success(result); }
    private async Task<string?> ScopeAsync() { var scope = await _scope.GetScopeAsync(); return scope.IsValid && !string.IsNullOrWhiteSpace(scope.AcademyId) ? scope.AcademyId : null; }
}
