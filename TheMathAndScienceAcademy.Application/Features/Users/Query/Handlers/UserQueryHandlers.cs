using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;
using TheMathAndScienceAcademy.Application.Features.Users.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Users.Query.Handlers;

public class UserQueryHandlers : ResponseHandler,
    IRequestHandler<GetUsersQuery, ApiResponse<List<UserDto>>>,
    IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>,
    IRequestHandler<GetUsersByAcademyQuery, ApiResponse<List<UserDto>>>
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private readonly IUserRepository _userRepository;
    private readonly IAcademyRepository _academyRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAcademyScopeService _academyScopeService;
    private readonly IMapper _mapper;

    public UserQueryHandlers(
        IUserRepository userRepository,
        IAcademyRepository academyRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService,
        IAcademyScopeService academyScopeService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _academyRepository = academyRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
        _academyScopeService = academyScopeService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<List<UserDto>>(ResponseMessages.Unauthorized);
        }

        if (!access.IsValid || string.IsNullOrWhiteSpace(access.AcademyId))
        {
            return BadRequest<List<UserDto>>(ResponseMessages.AcademySelectionRequired);
        }

        var users = await _userRepository.GetByAcademyIdAsync(access.AcademyId);

        return Success(await EnrichAsync(users));
    }

    public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<UserDto>(ResponseMessages.Unauthorized);
        }

        var user = await _userRepository.GetByIdAsync(request.Id.ToString());
        if (user is null)
        {
            return NotFound<UserDto>(ResponseMessages.UserNotFound);
        }

        if (!access.IsValid || string.IsNullOrWhiteSpace(access.AcademyId)
            || !string.Equals(user.AcademyId, access.AcademyId, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest<UserDto>(ResponseMessages.Forbidden);
        }

        return Success((await EnrichAsync(new[] { user })).Single());
    }

    public async Task<ApiResponse<List<UserDto>>> Handle(GetUsersByAcademyQuery request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<List<UserDto>>(ResponseMessages.Unauthorized);
        }

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId);
        if (academy is null)
        {
            return NotFound<List<UserDto>>(ResponseMessages.AcademyNotFound);
        }

        if (!access.IsValid || string.IsNullOrWhiteSpace(access.AcademyId)
            || !string.Equals(access.AcademyId, academy.Id, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest<List<UserDto>>(ResponseMessages.Forbidden);
        }

        var users = await _userRepository.GetByAcademyIdAsync(academy.Id);
        return Success(await EnrichAsync(users));
    }

    private async Task<UserAccessContext?> GetAccessContextAsync()
    {
        var scope = await _academyScopeService.GetScopeAsync();
        return scope.IsValid ? new UserAccessContext(scope.IsSuperAdmin, scope.AcademyId, scope.IsValid) : null;
    }

    private async Task<List<UserDto>> EnrichAsync(IEnumerable<TheMathAndScienceAcademy.Domain.Entities.User> users)
    {
        var roles = await _roleRepository.GetAllAsync();
        var academies = await _academyRepository.GetAllAsync();
        var roleLookup = roles.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
        var academyLookup = academies.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

        return users.Select(user =>
        {
            var dto = _mapper.Map<UserDto>(user);
            dto.RoleName = roleLookup.TryGetValue(user.RoleId, out var role) ? role.Name : null;
            dto.AcademyName = user.AcademyId is not null && academyLookup.TryGetValue(user.AcademyId, out var academy)
                ? academy.Name
                : null;
            return dto;
        }).ToList();
    }

    private sealed record UserAccessContext(bool IsSuperAdmin, string? AcademyId, bool IsValid);
}
