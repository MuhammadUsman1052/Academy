using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;

public class DeletePermissionCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
