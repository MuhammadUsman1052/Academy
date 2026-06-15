using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Permissions.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Query.Models;

public class GetPermissionByIdQuery : IRequest<ApiResponse<PermissionDto>>
{
    public Guid Id { get; set; }
}
