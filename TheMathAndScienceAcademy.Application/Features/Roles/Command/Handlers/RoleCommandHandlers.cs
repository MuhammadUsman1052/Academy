using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Handlers;

public class RoleCommandHandlers : ResponseHandler,
    IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>,
    IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>,
    IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
{
    private readonly IRoleRepository _repo;
    private readonly IMapper _mapper;

    public RoleCommandHandlers(IRoleRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repo.GetByNameAsync(request.Name);
        if (exists is not null)
            return BadRequest<RoleDto>(ResponseMessages.RoleAlreadyExists);

        var entity = _mapper.Map<TheMathAndScienceAcademy.Domain.Entities.Role>(request);
        var result = await _repo.CreateAsync(entity);

        if (result is null)
            return BadRequest<RoleDto>("Failed to create role");

        return Created(_mapper.Map<RoleDto>(result), ResponseMessages.RoleCreated);
    }

    public async Task<ApiResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByIdAsync(request.Id);
        if (existing is null)
            return NotFound<RoleDto>(ResponseMessages.RoleNotFound);

        _mapper.Map(request, existing);
        var updated = await _repo.UpdateAsync(existing);

        if (!updated)
            return BadRequest<RoleDto>("Role not updated");

        return Updated(_mapper.Map<RoleDto>(existing), ResponseMessages.RoleUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.DeleteAsync(request.Id);

        if (!ok)
            return NotFound<bool>(ResponseMessages.RoleNotFound);

        return Deleted<bool>(ResponseMessages.RoleDeleted);
    }
}
