using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Classes.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Classes.Query.Models;

public class GetAcademicClassesQuery : IRequest<ApiResponse<List<AcademicClassDto>>> { }

public class GetAcademicClassByIdQuery : IRequest<ApiResponse<AcademicClassDto>>
{
    public Guid Id { get; set; }
}
