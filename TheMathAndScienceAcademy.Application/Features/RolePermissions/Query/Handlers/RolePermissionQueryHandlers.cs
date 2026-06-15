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

    public RolePermissionQueryHandlers(
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RolePermissionDto>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<List<RolePermissionDto>>(ResponseMessages.RoleNotFound);
        }

        var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(role.Id);
        return Success(_mapper.Map<List<RolePermissionDto>>(permissions));
    }
}
