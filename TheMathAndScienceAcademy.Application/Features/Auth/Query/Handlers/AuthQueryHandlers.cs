using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;
using TheMathAndScienceAcademy.Application.Features.Auth.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Query.Handlers;

public class AuthQueryHandlers : ResponseHandler,
    IRequestHandler<GetCurrentUserQuery, ApiResponse<CurrentUserDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAcademyRepository _academyRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IMapper _mapper;

    public AuthQueryHandlers(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAcademyRepository academyRepository,
        IRolePermissionRepository rolePermissionRepository,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _academyRepository = academyRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return BadRequest<CurrentUserDto>(ResponseMessages.Unauthorized);

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
        if (user is null)
            return NotFound<CurrentUserDto>(ResponseMessages.UserNotFound);

        if (!user.IsActive)
            return BadRequest<CurrentUserDto>(ResponseMessages.UserInactive);

        if (!string.IsNullOrWhiteSpace(user.AcademyId)
            && (!Guid.TryParse(user.AcademyId, out var academyId)
                || await _academyRepository.GetByIdAsync(academyId) is null))
            return BadRequest<CurrentUserDto>(ResponseMessages.UserInactive);

        var dto = _mapper.Map<CurrentUserDto>(user);
        dto.RoleId = _currentUserService.RoleId ?? dto.RoleId;
        dto.AcademyId = _currentUserService.AcademyId ?? dto.AcademyId;
        if (Guid.TryParse(dto.RoleId, out var roleId))
        {
            dto.RoleName = (await _roleRepository.GetByIdAsync(roleId))?.Name ?? string.Empty;
        }
        if (Guid.TryParse(dto.AcademyId, out var academyIdValue))
        {
            dto.AcademyName = (await _academyRepository.GetByIdAsync(academyIdValue))?.Name;
        }
        dto.Permissions = (await _rolePermissionRepository.GetPermissionsByRoleIdAsync(dto.RoleId))
            .Where(x => x.IsGranted && x.Permission is not null)
            .Select(x => x.Permission!.Name)
            .ToList();

        return Success(dto);
    }
}
