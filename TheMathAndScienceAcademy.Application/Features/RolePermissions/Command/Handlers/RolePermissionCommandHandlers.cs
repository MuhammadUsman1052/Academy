using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Command.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Command.Handlers;

public class RolePermissionCommandHandlers : ResponseHandler,
    IRequestHandler<AssignPermissionToRoleCommand, ApiResponse<bool>>,
    IRequestHandler<RemovePermissionFromRoleCommand, ApiResponse<bool>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IAcademyScopeService _academyScopeService;
    private readonly ICurrentUserService _currentUserService;

    public RolePermissionCommandHandlers(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IAcademyScopeService academyScopeService,
        ICurrentUserService currentUserService)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _academyScopeService = academyScopeService;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<bool>(ResponseMessages.RoleNotFound);
        }

        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || (!scope.IsSuperAdmin && role.AcademyId is null)
            || (role.AcademyId is not null
                && !string.Equals(role.AcademyId, scope.AcademyId, StringComparison.OrdinalIgnoreCase)))
            return BadRequest<bool>(ResponseMessages.Forbidden);

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
        if (permission is null)
        {
            return NotFound<bool>(ResponseMessages.PermissionNotFound);
        }

        if (!scope.IsSuperAdmin && !await CurrentUserCanManagePermissionAsync(permission.Name))
            return BadRequest<bool>(ResponseMessages.Forbidden);

        var alreadyAssigned = await _rolePermissionRepository.RoleHasPermissionAsync(role.Id, permission.Name);
        if (alreadyAssigned)
        {
            return BadRequest<bool>(ResponseMessages.PermissionAlreadyAssigned);
        }

        var assigned = await _rolePermissionRepository.AssignPermissionAsync(role.Id, permission.Id);
        if (!assigned)
        {
            return BadRequest<bool>(ResponseMessages.PermissionAssignFailed);
        }

        return Created(true, ResponseMessages.PermissionAssigned);
    }

    public async Task<ApiResponse<bool>> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<bool>(ResponseMessages.RoleNotFound);
        }

        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || (!scope.IsSuperAdmin && role.AcademyId is null)
            || (role.AcademyId is not null
                && !string.Equals(role.AcademyId, scope.AcademyId, StringComparison.OrdinalIgnoreCase)))
            return BadRequest<bool>(ResponseMessages.Forbidden);

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
        if (permission is null)
        {
            return NotFound<bool>(ResponseMessages.PermissionNotFound);
        }

        if (!scope.IsSuperAdmin && !await CurrentUserCanManagePermissionAsync(permission.Name))
            return BadRequest<bool>(ResponseMessages.Forbidden);

        var removed = await _rolePermissionRepository.RemovePermissionAsync(role.Id, permission.Id);
        if (!removed)
        {
            return NotFound<bool>(ResponseMessages.PermissionNotFound);
        }

        return Deleted<bool>(ResponseMessages.PermissionRemoved);
    }

    private async Task<bool> CurrentUserCanManagePermissionAsync(string permissionName)
    {
        return !string.IsNullOrWhiteSpace(_currentUserService.RoleId)
            && await _rolePermissionRepository.RoleHasPermissionAsync(_currentUserService.RoleId, permissionName);
    }
}
