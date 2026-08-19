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
    private readonly IMapper _mapper;

    public UserQueryHandlers(
        IUserRepository userRepository,
        IAcademyRepository academyRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _academyRepository = academyRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<List<UserDto>>(ResponseMessages.Unauthorized);
        }

        if (!access.IsSuperAdmin && string.IsNullOrWhiteSpace(access.AcademyId))
        {
            return BadRequest<List<UserDto>>(ResponseMessages.Forbidden);
        }

        var users = access.IsSuperAdmin
            ? await _userRepository.GetAllAsync()
            : await _userRepository.GetByAcademyIdAsync(access.AcademyId!);

        return Success(_mapper.Map<List<UserDto>>(users));
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

        if (!access.IsSuperAdmin && !string.Equals(user.AcademyId, access.AcademyId, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest<UserDto>(ResponseMessages.Forbidden);
        }

        return Success(_mapper.Map<UserDto>(user));
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

        if (!access.IsSuperAdmin && !string.Equals(access.AcademyId, academy.Id, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest<List<UserDto>>(ResponseMessages.Forbidden);
        }

        var users = await _userRepository.GetByAcademyIdAsync(academy.Id);
        return Success(_mapper.Map<List<UserDto>>(users));
    }

    private async Task<UserAccessContext?> GetAccessContextAsync()
    {
        if (!Guid.TryParse(_currentUserService.RoleId, out var roleId))
        {
            return null;
        }

        var currentRole = await _roleRepository.GetByIdAsync(roleId);
        if (currentRole is null)
        {
            return null;
        }

        var isSuperAdmin = string.Equals(currentRole.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase);
        var academyId = string.IsNullOrWhiteSpace(_currentUserService.AcademyId)
            ? currentRole.AcademyId
            : _currentUserService.AcademyId;

        return new UserAccessContext(isSuperAdmin, academyId);
    }

    private sealed record UserAccessContext(bool IsSuperAdmin, string? AcademyId);
}
