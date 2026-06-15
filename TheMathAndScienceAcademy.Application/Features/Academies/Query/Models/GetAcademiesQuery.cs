using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Query.Models;

public class GetAcademiesQuery : IRequest<ApiResponse<List<AcademyDto>>>
{
}