using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Permissions.Dtos;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Command.Handlers;

public class PermissionCommandHandlers : ResponseHandler,
    IRequestHandler<UpdatePermissionCommand, ApiResponse<PermissionDto>>,
    IRequestHandler<DeletePermissionCommand, ApiResponse<bool>>
{
    private readonly IPermissionRepository _repo;
    private readonly IMapper _mapper;

    public PermissionCommandHandlers(IPermissionRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PermissionDto>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByIdAsync(request.Id);
        if (existing is null)
            return NotFound<PermissionDto>(ResponseMessages.PermissionNotFound);

        var duplicateByName = await _repo.GetByNameAsync(request.Name);
        if (duplicateByName is not null && duplicateByName.Id != existing.Id)
            return BadRequest<PermissionDto>(ResponseMessages.PermissionAlreadyExists);

        _mapper.Map(request, existing);
        var updated = await _repo.UpdateAsync(existing);

        if (!updated)
            return BadRequest<PermissionDto>(ResponseMessages.PermissionUpdateFailed);

        return Updated(_mapper.Map<PermissionDto>(existing), ResponseMessages.PermissionUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.DeleteAsync(request.Id);

        if (!ok)
            return NotFound<bool>(ResponseMessages.PermissionNotFound);

        return Deleted<bool>(ResponseMessages.PermissionDeleted);
    }
}
