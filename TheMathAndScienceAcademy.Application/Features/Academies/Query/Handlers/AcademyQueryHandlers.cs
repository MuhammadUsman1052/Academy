using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;
using TheMathAndScienceAcademy.Application.Features.Academies.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Query.Handlers;

public class AcademyQueryHandlers : ResponseHandler,
    IRequestHandler<GetAcademiesQuery, ApiResponse<List<AcademyDto>>>,
    IRequestHandler<GetAcademyByIdQuery, ApiResponse<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyScopeService _academyScopeService;
    private readonly IMapper _mapper;

    public AcademyQueryHandlers(IAcademyRepository academyRepository, IAcademyScopeService academyScopeService, IMapper mapper)
    {
        _academyRepository = academyRepository;
        _academyScopeService = academyScopeService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<AcademyDto>>> Handle(GetAcademiesQuery request, CancellationToken cancellationToken)
    {
        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid)
            return BadRequest<List<AcademyDto>>(ResponseMessages.Unauthorized);

        var academies = scope.IsSuperAdmin
            ? await _academyRepository.GetAllAsync()
            : string.IsNullOrWhiteSpace(scope.AcademyId)
                ? new List<Domain.Entities.Academy>()
                : new List<Domain.Entities.Academy>
                {
                    (await _academyRepository.GetByIdAsync(Guid.Parse(scope.AcademyId)))!
                };
        academies = academies.Where(academy => academy is not null).ToList();
        return Success(_mapper.Map<List<AcademyDto>>(academies));
    }

    public async Task<ApiResponse<AcademyDto>> Handle(GetAcademyByIdQuery request, CancellationToken cancellationToken)
    {
        var academy = await _academyRepository.GetByIdAsync(request.Id);

        if (academy is null)
        {
            return NotFound<AcademyDto>(ResponseMessages.AcademyNotFound);
        }

        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || (!scope.IsSuperAdmin
            && !string.Equals(scope.AcademyId, academy.Id, StringComparison.OrdinalIgnoreCase)))
            return BadRequest<AcademyDto>(ResponseMessages.Forbidden);

        return Success(_mapper.Map<AcademyDto>(academy));
    }
}
