using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Query.Models;

public class GetRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
{
}
