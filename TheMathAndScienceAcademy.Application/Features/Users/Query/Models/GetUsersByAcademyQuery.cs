using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Users.Query.Models;

public class GetUsersByAcademyQuery : IRequest<ApiResponse<List<UserDto>>>
{
    public Guid AcademyId { get; set; }
}
