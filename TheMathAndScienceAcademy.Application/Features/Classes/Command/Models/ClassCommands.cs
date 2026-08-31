using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Classes.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Classes.Command.Models;

public class CreateAcademicClassCommand : IRequest<ApiResponse<AcademicClassDto>>
{
    public string Name { get; set; } = default!;
    public string Grade { get; set; } = default!;
    public string Section { get; set; } = default!;
    public string AcademicYear { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? AcademyId { get; set; }
}

public class UpdateAcademicClassCommand : CreateAcademicClassCommand, IRequest<ApiResponse<AcademicClassDto>>
{
    public Guid Id { get; set; }
}

public class DeleteAcademicClassCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
