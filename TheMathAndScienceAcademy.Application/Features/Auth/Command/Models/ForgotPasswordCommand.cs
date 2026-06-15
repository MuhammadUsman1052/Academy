using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

public class ForgotPasswordCommand : IRequest<ApiResponse<bool>>
{
    public string Email { get; set; } = default!;
}