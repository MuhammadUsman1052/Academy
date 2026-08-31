using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Classes.Dtos;
using TheMathAndScienceAcademy.Application.Features.Classes.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Classes.Query.Handlers;

public class ClassQueryHandlers : ResponseHandler, IRequestHandler<GetAcademicClassesQuery, ApiResponse<List<AcademicClassDto>>>, IRequestHandler<GetAcademicClassByIdQuery, ApiResponse<AcademicClassDto>>
{
    private readonly IAcademicClassRepository _classes; private readonly IAcademyRepository _academies; private readonly IAcademyScopeService _scope; private readonly IMapper _mapper;
    public ClassQueryHandlers(IAcademicClassRepository classes, IAcademyRepository academies, IAcademyScopeService scope, IMapper mapper) { _classes = classes; _academies = academies; _scope = scope; _mapper = mapper; }
    public async Task<ApiResponse<List<AcademicClassDto>>> Handle(GetAcademicClassesQuery request, CancellationToken cancellationToken)
    { var scope = await _scope.GetScopeAsync(); if (!scope.IsValid || string.IsNullOrWhiteSpace(scope.AcademyId)) return BadRequest<List<AcademicClassDto>>(ResponseMessages.AcademySelectionRequired); return Success(await ToDtosAsync(await _classes.GetByAcademyIdAsync(scope.AcademyId))); }
    public async Task<ApiResponse<AcademicClassDto>> Handle(GetAcademicClassByIdQuery request, CancellationToken cancellationToken)
    { var scope = await _scope.GetScopeAsync(); var item = await _classes.GetByIdAsync(request.Id.ToString()); if (item is null) return NotFound<AcademicClassDto>(ResponseMessages.ClassNotFound); if (!scope.IsValid || !string.Equals(scope.AcademyId, item.AcademyId, StringComparison.OrdinalIgnoreCase)) return BadRequest<AcademicClassDto>(ResponseMessages.Forbidden); return Success((await ToDtosAsync([item]))[0]); }
    private async Task<List<AcademicClassDto>> ToDtosAsync(IEnumerable<Domain.Entities.AcademicClass> items) { var result = new List<AcademicClassDto>(); foreach (var item in items) { var dto = _mapper.Map<AcademicClassDto>(item); dto.AcademyName = (await _academies.GetByIdAsync(Guid.Parse(item.AcademyId)))?.Name; result.Add(dto); } return result; }
}
