using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Handlers;

public class RoleCommandHandlers : ResponseHandler,
    IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>,
    IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>,
    IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
{
    private readonly IRoleRepository _repo;
    private readonly IRolePermissionSyncService _rolePermissionSyncService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public RoleCommandHandlers(
        IRoleRepository repo,
        IRolePermissionSyncService rolePermissionSyncService,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _repo = repo;
        _rolePermissionSyncService = rolePermissionSyncService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId) || string.IsNullOrWhiteSpace(_currentUserService.RoleId))
            return BadRequest<RoleDto>(ResponseMessages.Unauthorized);

        var currentUserRole = await _repo.GetByIdAsync(Guid.Parse(_currentUserService.RoleId));
        if (currentUserRole is null)
            return BadRequest<RoleDto>(ResponseMessages.Unauthorized);

        var isSuperAdmin = string.Equals(currentUserRole.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        var currentAcademyId = string.IsNullOrWhiteSpace(_currentUserService.AcademyId) ? null : _currentUserService.AcademyId;

        var targetAcademyId = request.AcademyId?.ToString();
        if (!isSuperAdmin)
        {
            if (string.IsNullOrWhiteSpace(currentAcademyId))
                return BadRequest<RoleDto>(ResponseMessages.Forbidden);

            if (request.AcademyId is not null && !string.Equals(targetAcademyId, currentAcademyId, StringComparison.OrdinalIgnoreCase))
                return BadRequest<RoleDto>(ResponseMessages.Forbidden);

            targetAcademyId = currentAcademyId;
        }

        var exists = await _repo.GetByNameAsync(request.Name, targetAcademyId);
        if (exists is not null)
            return BadRequest<RoleDto>(ResponseMessages.RoleAlreadyExists);

        var entity = _mapper.Map<Role>(request);
        entity.AcademyId = targetAcademyId;

        var result = await _repo.CreateAsync(entity);

        if (result is null)
            return BadRequest<RoleDto>(ResponseMessages.RoleCreateFailed);

        await _rolePermissionSyncService.SyncRoleAsync(result.Id);
        return Created(_mapper.Map<RoleDto>(result), ResponseMessages.RoleCreated);
    }

    public async Task<ApiResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByIdAsync(request.Id);
        if (existing is null)
            return NotFound<RoleDto>(ResponseMessages.RoleNotFound);

        if (string.IsNullOrWhiteSpace(_currentUserService.UserId) || string.IsNullOrWhiteSpace(_currentUserService.RoleId))
            return BadRequest<RoleDto>(ResponseMessages.Unauthorized);

        var currentUserRole = await _repo.GetByIdAsync(Guid.Parse(_currentUserService.RoleId));
        if (currentUserRole is null)
            return BadRequest<RoleDto>(ResponseMessages.Unauthorized);

        var isSuperAdmin = string.Equals(currentUserRole.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        var currentAcademyId = string.IsNullOrWhiteSpace(_currentUserService.AcademyId) ? null : _currentUserService.AcademyId;
        if (!isSuperAdmin && !string.Equals(existing.AcademyId, currentAcademyId, StringComparison.OrdinalIgnoreCase))
            return BadRequest<RoleDto>(ResponseMessages.Forbidden);

        var requestedAcademyId = request.AcademyId?.ToString() ?? existing.AcademyId;
        if (!isSuperAdmin)
        {
            if (string.IsNullOrWhiteSpace(currentAcademyId))
                return BadRequest<RoleDto>(ResponseMessages.Forbidden);

            if (!string.Equals(requestedAcademyId, currentAcademyId, StringComparison.OrdinalIgnoreCase))
                return BadRequest<RoleDto>(ResponseMessages.Forbidden);
        }

        _mapper.Map(request, existing);
        existing.AcademyId = requestedAcademyId;
        var updated = await _repo.UpdateAsync(existing);

        if (!updated)
            return BadRequest<RoleDto>(ResponseMessages.RoleUpdateFailed);

        return Updated(_mapper.Map<RoleDto>(existing), ResponseMessages.RoleUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId) || string.IsNullOrWhiteSpace(_currentUserService.RoleId))
            return BadRequest<bool>(ResponseMessages.Unauthorized);

        var currentUserRole = await _repo.GetByIdAsync(Guid.Parse(_currentUserService.RoleId));
        if (currentUserRole is null)
            return BadRequest<bool>(ResponseMessages.Unauthorized);

        var existing = await _repo.GetByIdAsync(request.Id);
        if (existing is null)
            return NotFound<bool>(ResponseMessages.RoleNotFound);

        var isSuperAdmin = string.Equals(currentUserRole.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        var currentAcademyId = string.IsNullOrWhiteSpace(_currentUserService.AcademyId) ? null : _currentUserService.AcademyId;
        if (!isSuperAdmin && !string.Equals(existing.AcademyId, currentAcademyId, StringComparison.OrdinalIgnoreCase))
            return BadRequest<bool>(ResponseMessages.Forbidden);

        var ok = await _repo.DeleteAsync(request.Id);

        if (!ok)
            return NotFound<bool>(ResponseMessages.RoleNotFound);

        return Deleted<bool>(ResponseMessages.RoleDeleted);
    }
}
