using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Query.Models;

public class GetAcademyByIdQuery : IRequest<ApiResponse<AcademyDto>>
{
    public Guid Id { get; set; }
}