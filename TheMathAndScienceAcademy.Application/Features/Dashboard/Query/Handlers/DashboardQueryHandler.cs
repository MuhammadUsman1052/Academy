using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Dashboard.Dtos;
using TheMathAndScienceAcademy.Application.Features.Dashboard.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Dashboard.Query.Handlers;

public sealed class DashboardQueryHandler : ResponseHandler, IRequestHandler<GetDashboardSummaryQuery, ApiResponse<DashboardSummaryDto>>
{
    private readonly IAcademyScopeService _scopeService;
    private readonly IAcademyRepository _academyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IAcademicClassRepository _classRepository;
    private readonly IStudentEnrollmentRepository _enrollmentRepository;

    public DashboardQueryHandler(IAcademyScopeService scopeService, IAcademyRepository academyRepository,
        IUserRepository userRepository, IStudentRepository studentRepository,
        IAcademicClassRepository classRepository, IStudentEnrollmentRepository enrollmentRepository)
    {
        _scopeService = scopeService;
        _academyRepository = academyRepository;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _classRepository = classRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<ApiResponse<DashboardSummaryDto>> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var scope = await _scopeService.GetScopeAsync();
        if (!scope.IsValid)
            return BadRequest<DashboardSummaryDto>(ResponseMessages.Unauthorized);

        var summary = new DashboardSummaryDto
        {
            TotalAcademies = (await _academyRepository.GetAllAsync()).Count
        };

        if (string.IsNullOrWhiteSpace(scope.AcademyId))
            return Success(summary);

        var academy = Guid.TryParse(scope.AcademyId, out var academyId)
            ? await _academyRepository.GetByIdAsync(academyId)
            : null;
        if (academy is null)
            return BadRequest<DashboardSummaryDto>(ResponseMessages.AcademySelectionRequired);

        summary.AcademyId = academy.Id;
        summary.AcademyName = academy.Name;
        summary.UserCount = (await _userRepository.GetByAcademyIdAsync(academy.Id)).Count;
        summary.StudentCount = (await _studentRepository.GetByAcademyIdAsync(academy.Id)).Count;
        summary.ClassCount = (await _classRepository.GetByAcademyIdAsync(academy.Id)).Count;
        summary.EnrollmentCount = (await _enrollmentRepository.GetByAcademyIdAsync(academy.Id)).Count;

        return Success(summary);
    }
}
