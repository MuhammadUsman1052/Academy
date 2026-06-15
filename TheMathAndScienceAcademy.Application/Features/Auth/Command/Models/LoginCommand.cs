using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

public class LoginCommand : IRequest<ApiResponse<LoginResponseDto>>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
