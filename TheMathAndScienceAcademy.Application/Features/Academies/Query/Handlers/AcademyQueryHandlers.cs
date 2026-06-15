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
    private readonly IMapper _mapper;

    public AcademyQueryHandlers(IAcademyRepository academyRepository, IMapper mapper)
    {
        _academyRepository = academyRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<AcademyDto>>> Handle(GetAcademiesQuery request, CancellationToken cancellationToken)
    {
        var academies = await _academyRepository.GetAllAsync();
        return Success(_mapper.Map<List<AcademyDto>>(academies));
    }

    public async Task<ApiResponse<AcademyDto>> Handle(GetAcademyByIdQuery request, CancellationToken cancellationToken)
    {
        var academy = await _academyRepository.GetByIdAsync(request.Id);

        if (academy is null)
        {
            return NotFound<AcademyDto>(ResponseMessages.AcademyNotFound);
        }

        return Success(_mapper.Map<AcademyDto>(academy));
    }
}