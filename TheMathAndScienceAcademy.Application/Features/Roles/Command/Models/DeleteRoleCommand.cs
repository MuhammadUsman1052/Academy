using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

public class DeleteRoleCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
