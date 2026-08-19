using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Users.Command.Models;

public class DeleteUserCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
