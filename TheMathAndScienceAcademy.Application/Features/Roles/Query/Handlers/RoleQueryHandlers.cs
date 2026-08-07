using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;
using TheMathAndScienceAcademy.Application.Features.Roles.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Query.Handlers;

public class RoleQueryHandlers : ResponseHandler,
    IRequestHandler<GetRolesQuery, ApiResponse<List<RoleDto>>>,
    IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDto>>
{
    private readonly IRoleRepository _repo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public RoleQueryHandlers(IRoleRepository repo, ICurrentUserService currentUserService, IMapper mapper)
    {
        _repo = repo;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _repo.GetAllAsync();
        var currentRole = await GetCurrentRoleAsync();

        if (currentRole is not null && !string.Equals(currentRole.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
        {
            roles = roles.Where(role => string.Equals(role.AcademyId, _currentUserService.AcademyId, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Success(_mapper.Map<List<RoleDto>>(roles));
    }

    public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _repo.GetByIdAsync(request.Id);

        if (role is null)
            return NotFound<RoleDto>(ResponseMessages.RoleNotFound);

        var currentRole = await GetCurrentRoleAsync();
        if (currentRole is not null && !string.Equals(currentRole.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(role.AcademyId, _currentUserService.AcademyId, StringComparison.OrdinalIgnoreCase))
                return BadRequest<RoleDto>(ResponseMessages.Forbidden);
        }

        return Success(_mapper.Map<RoleDto>(role));
    }

    private async Task<Domain.Entities.Role?> GetCurrentRoleAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.RoleId))
        {
            return null;
        }

        return await _repo.GetByIdAsync(Guid.Parse(_currentUserService.RoleId));
    }
}
