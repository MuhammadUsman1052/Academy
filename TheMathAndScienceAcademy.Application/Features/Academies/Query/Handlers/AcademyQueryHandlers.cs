using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;
using TheMathAndScienceAcademy.Application.Features.Academies.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Query.Handlers;

public class AcademyQueryHandlers : ResponseHandler,
    IRequestHandler<GetAcademiesQuery, ApiResponse<List<AcademyDto>>>,
    IRequestHandler<GetAcademyByIdQuery, ApiResponse<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyScopeService _academyScopeService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IAcademicClassRepository _classRepository;
    private readonly IStudentEnrollmentRepository _enrollmentRepository;

    public AcademyQueryHandlers(IAcademyRepository academyRepository, IAcademyScopeService academyScopeService, IMapper mapper,
        IUserRepository userRepository, IRoleRepository roleRepository, IStudentRepository studentRepository,
        IAcademicClassRepository classRepository, IStudentEnrollmentRepository enrollmentRepository)
    {
        _academyRepository = academyRepository;
        _academyScopeService = academyScopeService;
        _mapper = mapper;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _studentRepository = studentRepository;
        _classRepository = classRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<ApiResponse<List<AcademyDto>>> Handle(GetAcademiesQuery request, CancellationToken cancellationToken)
    {
        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid)
            return BadRequest<List<AcademyDto>>(ResponseMessages.Unauthorized);

        var academies = scope.IsSuperAdmin
            ? await _academyRepository.GetAllAsync()
            : string.IsNullOrWhiteSpace(scope.AcademyId)
                ? new List<Domain.Entities.Academy>()
                : new List<Domain.Entities.Academy>
                {
                    (await _academyRepository.GetByIdAsync(Guid.Parse(scope.AcademyId)))!
                };
        academies = academies.Where(academy => academy is not null).ToList();
        return Success(await EnrichAsync(academies));
    }

    public async Task<ApiResponse<AcademyDto>> Handle(GetAcademyByIdQuery request, CancellationToken cancellationToken)
    {
        var academy = await _academyRepository.GetByIdAsync(request.Id);

        if (academy is null)
        {
            return NotFound<AcademyDto>(ResponseMessages.AcademyNotFound);
        }

        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || (!scope.IsSuperAdmin
            && !string.Equals(scope.AcademyId, academy.Id, StringComparison.OrdinalIgnoreCase)))
            return BadRequest<AcademyDto>(ResponseMessages.Forbidden);

        return Success((await EnrichAsync(new[] { academy }))[0]);
    }

    private async Task<List<AcademyDto>> EnrichAsync(IEnumerable<Domain.Entities.Academy> academies)
    {
        var roles = await _roleRepository.GetAllAsync();
        var result = new List<AcademyDto>();
        foreach (var academy in academies)
        {
            var dto = _mapper.Map<AcademyDto>(academy);
            var users = await _userRepository.GetByAcademyIdAsync(academy.Id);
            var students = await _studentRepository.GetByAcademyIdAsync(academy.Id);
            var classes = await _classRepository.GetByAcademyIdAsync(academy.Id);
            var enrollments = await _enrollmentRepository.GetByAcademyIdAsync(academy.Id);
            var adminRole = roles.FirstOrDefault(x => string.Equals(x.Name, "AcademyAdmin", StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.AcademyId, academy.Id, StringComparison.OrdinalIgnoreCase));
            var admin = users.FirstOrDefault(x => x.RoleId == adminRole?.Id);
            dto.AdminRoleId = adminRole is null ? null : Guid.Parse(adminRole.Id);
            dto.AdminUserId = admin is null ? null : Guid.Parse(admin.Id);
            dto.AdminRoleName = adminRole?.Name;
            dto.AdminName = admin?.Name;
            dto.AdminEmail = admin?.Email;
            dto.UserCount = users.Count;
            dto.StudentCount = students.Count;
            dto.ClassCount = classes.Count;
            dto.EnrollmentCount = enrollments.Count;
            result.Add(dto);
        }
        return result;
    }
}
