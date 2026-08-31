using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;
using TheMathAndScienceAcademy.Application.Features.Students.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Students.Query.Handlers;

public class StudentQueryHandlers : ResponseHandler,
    IRequestHandler<GetStudentsQuery, ApiResponse<List<StudentDto>>>,
    IRequestHandler<GetStudentByIdQuery, ApiResponse<StudentDto>>,
    IRequestHandler<GetStudentsByAcademyQuery, ApiResponse<List<StudentDto>>>,
    IRequestHandler<GetMyStudentProfileQuery, ApiResponse<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IAcademyRepository _academyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAcademyScopeService _scopeService;
    private readonly IMapper _mapper;

    public StudentQueryHandlers(
        IStudentRepository studentRepository,
        IAcademyRepository academyRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IAcademyScopeService scopeService,
        IMapper mapper)
    {
        _studentRepository = studentRepository;
        _academyRepository = academyRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _scopeService = scopeService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var scope = await _scopeService.GetScopeAsync();
        if (!scope.IsValid || string.IsNullOrWhiteSpace(scope.AcademyId))
            return BadRequest<List<StudentDto>>(ResponseMessages.AcademySelectionRequired);
        return Success(await ToDtosAsync(await _studentRepository.GetByAcademyIdAsync(scope.AcademyId)));
    }

    public async Task<ApiResponse<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var scope = await _scopeService.GetScopeAsync();
        var student = await _studentRepository.GetByIdAsync(request.Id.ToString());
        if (student is null) return NotFound<StudentDto>(ResponseMessages.StudentNotFound);
        if (!CanAccess(scope, student.AcademyId)) return BadRequest<StudentDto>(ResponseMessages.Forbidden);
        var items = await ToDtosAsync(new[] { student });
        return Success(items[0]);
    }

    public async Task<ApiResponse<List<StudentDto>>> Handle(GetStudentsByAcademyQuery request, CancellationToken cancellationToken)
    {
        var scope = await _scopeService.GetScopeAsync();
        if (!CanAccess(scope, request.AcademyId.ToString())) return BadRequest<List<StudentDto>>(ResponseMessages.Forbidden);
        if (await _academyRepository.GetByIdAsync(request.AcademyId) is null) return NotFound<List<StudentDto>>(ResponseMessages.AcademyNotFound);
        return Success(await ToDtosAsync(await _studentRepository.GetByAcademyIdAsync(request.AcademyId.ToString())));
    }

    public async Task<ApiResponse<StudentDto>> Handle(GetMyStudentProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return BadRequest<StudentDto>(ResponseMessages.Unauthorized);
        var student = await _studentRepository.GetByUserIdAsync(_currentUserService.UserId);
        if (student is null) return NotFound<StudentDto>(ResponseMessages.StudentNotLinked);
        var scope = await _scopeService.GetScopeAsync();
        if (!CanAccess(scope, student.AcademyId)) return BadRequest<StudentDto>(ResponseMessages.Forbidden);
        var items = await ToDtosAsync(new[] { student });
        return Success(items[0], ResponseMessages.StudentProfileLoaded);
    }

    private static bool CanAccess(AcademyScope scope, string academyId)
        => scope.IsValid && !string.IsNullOrWhiteSpace(scope.AcademyId) && string.Equals(scope.AcademyId, academyId, StringComparison.OrdinalIgnoreCase);

    private async Task<List<StudentDto>> ToDtosAsync(IEnumerable<Domain.Entities.Student> students)
    {
        var result = new List<StudentDto>();
        foreach (var student in students)
        {
            var dto = _mapper.Map<StudentDto>(student);
            dto.AcademyName = (await _academyRepository.GetByIdAsync(Guid.Parse(student.AcademyId)))?.Name;
            var user = student.UserId is null ? null : await _userRepository.GetByIdAsync(student.UserId);
            dto.UserName = user?.Name;
            dto.LoginEnabled = user?.IsActive == true;
            dto.MustChangePassword = user?.MustChangePassword == true;
            dto.CredentialsSentAt = user?.CredentialsSentAt;
            dto.CredentialsPending = user is not null && user.CredentialsSentAt is null;
            result.Add(dto);
        }
        return result;
    }
}
