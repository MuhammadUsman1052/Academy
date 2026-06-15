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
    private readonly IMapper _mapper;

    public RoleQueryHandlers(IRoleRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _repo.GetAllAsync();
        return Success(_mapper.Map<List<RoleDto>>(roles));
    }

    public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _repo.GetByIdAsync(request.Id);

        if (role is null)
            return NotFound<RoleDto>(ResponseMessages.RoleNotFound);

        return Success(_mapper.Map<RoleDto>(role));
    }
}
