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
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyScopeService _academyScopeService;
    private readonly IMapper _mapper;

    public RoleQueryHandlers(IRoleRepository repo, IAcademyRepository academyRepository, IAcademyScopeService academyScopeService, IMapper mapper)
    {
        _repo = repo;
        _academyRepository = academyRepository;
        _academyScopeService = academyScopeService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _repo.GetAllAsync();
        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid)
            return BadRequest<List<RoleDto>>(ResponseMessages.Unauthorized);

        roles = roles.Where(role =>
            (scope.IsSuperAdmin && role.AcademyId is null)
            || (scope.IsSuperAdmin && !string.IsNullOrWhiteSpace(scope.AcademyId)
                && string.Equals(role.AcademyId, scope.AcademyId, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrWhiteSpace(scope.AcademyId)
                && string.Equals(role.AcademyId, scope.AcademyId, StringComparison.OrdinalIgnoreCase))).ToList();

        var academies = await _academyRepository.GetAllAsync();
        var academyLookup = academies.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
        var result = _mapper.Map<List<RoleDto>>(roles);
        foreach (var dto in result)
            dto.AcademyName = dto.AcademyId.HasValue && academyLookup.TryGetValue(dto.AcademyId.Value.ToString(), out var academy) ? academy.Name : null;
        return Success(result);
    }

    public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _repo.GetByIdAsync(request.Id);

        if (role is null)
            return NotFound<RoleDto>(ResponseMessages.RoleNotFound);

        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || (!scope.IsSuperAdmin && role.AcademyId is null)
            || (role.AcademyId is not null
                && !string.Equals(role.AcademyId, scope.AcademyId, StringComparison.OrdinalIgnoreCase)))
            return BadRequest<RoleDto>(ResponseMessages.Forbidden);

        var dto = _mapper.Map<RoleDto>(role);
        dto.AcademyName = role.AcademyId is not null && Guid.TryParse(role.AcademyId, out var academyId)
            ? (await _academyRepository.GetByIdAsync(academyId))?.Name
            : null;
        return Success(dto);
    }

}
