using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Classes.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Classes.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Classes.Command.Handlers;

public class ClassCommandHandlers : ResponseHandler,
    IRequestHandler<CreateAcademicClassCommand, ApiResponse<AcademicClassDto>>,
    IRequestHandler<UpdateAcademicClassCommand, ApiResponse<AcademicClassDto>>,
    IRequestHandler<DeleteAcademicClassCommand, ApiResponse<bool>>
{
    private readonly IAcademicClassRepository _classes;
    private readonly IAcademyRepository _academies;
    private readonly IAcademyScopeService _scope;
    private readonly IMapper _mapper;

    public ClassCommandHandlers(IAcademicClassRepository classes, IAcademyRepository academies, IAcademyScopeService scope, IMapper mapper)
    { _classes = classes; _academies = academies; _scope = scope; _mapper = mapper; }

    public async Task<ApiResponse<AcademicClassDto>> Handle(CreateAcademicClassCommand request, CancellationToken cancellationToken)
    {
        var academyId = await ResolveAcademyAsync(request.AcademyId);
        if (academyId is null) return BadRequest<AcademicClassDto>(ResponseMessages.AcademySelectionRequired);
        if (await _academies.GetByIdAsync(Guid.Parse(academyId)) is null) return NotFound<AcademicClassDto>(ResponseMessages.AcademyNotFound);
        if (await _classes.GetDuplicateAsync(academyId, request.Grade.Trim(), request.Section.Trim(), request.AcademicYear.Trim()) is not null)
            return BadRequest<AcademicClassDto>(ResponseMessages.ClassAlreadyExists);
        var item = new AcademicClass { Name = request.Name.Trim(), Grade = request.Grade.Trim(), Section = request.Section.Trim(), AcademicYear = request.AcademicYear.Trim(), Description = Normalize(request.Description), Status = NormalizeStatus(request.Status), AcademyId = academyId };
        var created = await _classes.CreateAsync(item);
        return created is null ? BadRequest<AcademicClassDto>(ResponseMessages.ClassCreateFailed) : Created(await ToDtoAsync(created), ResponseMessages.ClassCreated);
    }

    public async Task<ApiResponse<AcademicClassDto>> Handle(UpdateAcademicClassCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync();
        var item = await _classes.GetByIdAsync(request.Id.ToString());
        if (item is null) return NotFound<AcademicClassDto>(ResponseMessages.ClassNotFound);
        if (!CanAccess(scope, item.AcademyId)) return BadRequest<AcademicClassDto>(ResponseMessages.Forbidden);
        if (await _classes.GetDuplicateAsync(item.AcademyId, request.Grade.Trim(), request.Section.Trim(), request.AcademicYear.Trim(), item.Id) is not null)
            return BadRequest<AcademicClassDto>(ResponseMessages.ClassAlreadyExists);
        item.Name = request.Name.Trim(); item.Grade = request.Grade.Trim(); item.Section = request.Section.Trim(); item.AcademicYear = request.AcademicYear.Trim(); item.Description = Normalize(request.Description); item.Status = NormalizeStatus(request.Status); item.UpdatedAt = DateTime.UtcNow;
        if (!await _classes.UpdateAsync(item)) return BadRequest<AcademicClassDto>(ResponseMessages.ClassUpdateFailed);
        return Updated(await ToDtoAsync(item), ResponseMessages.ClassUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeleteAcademicClassCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync();
        var item = await _classes.GetByIdAsync(request.Id.ToString());
        if (item is null) return NotFound<bool>(ResponseMessages.ClassNotFound);
        if (!CanAccess(scope, item.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden);
        if (!await _classes.DeleteAsync(item.Id)) return NotFound<bool>(ResponseMessages.ClassNotFound);
        return Deleted<bool>(ResponseMessages.ClassDeleted);
    }

    private async Task<string?> ResolveAcademyAsync(Guid? requested)
    { var scope = await _scope.GetScopeAsync(); if (!scope.IsValid || string.IsNullOrWhiteSpace(scope.AcademyId)) return null; if (requested.HasValue && !requested.Value.ToString().Equals(scope.AcademyId, StringComparison.OrdinalIgnoreCase)) return null; return scope.AcademyId; }
    private static bool CanAccess(AcademyScope scope, string academyId) => scope.IsValid && string.Equals(scope.AcademyId, academyId, StringComparison.OrdinalIgnoreCase);
    private async Task<AcademicClassDto> ToDtoAsync(AcademicClass item) { var dto = _mapper.Map<AcademicClassDto>(item); dto.AcademyName = (await _academies.GetByIdAsync(Guid.Parse(item.AcademyId)))?.Name; return dto; }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string NormalizeStatus(string value) => string.Equals(value, "Inactive", StringComparison.OrdinalIgnoreCase) ? "Inactive" : "Active";
}
