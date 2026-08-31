using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Dtos;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Query.Handlers;

public class RolePermissionQueryHandlers : ResponseHandler,
    IRequestHandler<GetRolePermissionsQuery, ApiResponse<List<RolePermissionDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IMapper _mapper;
    private readonly IAcademyScopeService _academyScopeService;

    public RolePermissionQueryHandlers(
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IMapper mapper,
        IAcademyScopeService academyScopeService)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _mapper = mapper;
        _academyScopeService = academyScopeService;
    }

    public async Task<ApiResponse<List<RolePermissionDto>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<List<RolePermissionDto>>(ResponseMessages.RoleNotFound);
        }

        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || (!scope.IsSuperAdmin && role.AcademyId is null)
            || (role.AcademyId is not null
                && !string.Equals(role.AcademyId, scope.AcademyId, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest<List<RolePermissionDto>>(ResponseMessages.Forbidden);
        }

        var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(role.Id);
        return Success(_mapper.Map<List<RolePermissionDto>>(permissions));
    }
}
