using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Permissions.Dtos;
using TheMathAndScienceAcademy.Application.Features.Permissions.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Query.Handlers;

public class PermissionQueryHandlers : ResponseHandler,
    IRequestHandler<GetPermissionsQuery, ApiResponse<List<PermissionDto>>>,
    IRequestHandler<GetPermissionByIdQuery, ApiResponse<PermissionDto>>
{
    private readonly IPermissionRepository _repo;
    private readonly IMapper _mapper;

    public PermissionQueryHandlers(IPermissionRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _repo.GetAllAsync();
        return Success(_mapper.Map<List<PermissionDto>>(permissions));
    }

    public async Task<ApiResponse<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var permission = await _repo.GetByIdAsync(request.Id);

        if (permission is null)
            return NotFound<PermissionDto>(ResponseMessages.PermissionNotFound);

        return Success(_mapper.Map<PermissionDto>(permission));
    }
}
