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

    public RolePermissionCommandHandlers(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<ApiResponse<bool>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<bool>(ResponseMessages.RoleNotFound);
        }

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
        if (permission is null)
        {
            return NotFound<bool>(ResponseMessages.PermissionNotFound);
        }

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

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
        if (permission is null)
        {
            return NotFound<bool>(ResponseMessages.PermissionNotFound);
        }

        var removed = await _rolePermissionRepository.RemovePermissionAsync(role.Id, permission.Id);
        if (!removed)
        {
            return NotFound<bool>(ResponseMessages.PermissionNotFound);
        }

        return Deleted<bool>(ResponseMessages.PermissionRemoved);
    }
}
