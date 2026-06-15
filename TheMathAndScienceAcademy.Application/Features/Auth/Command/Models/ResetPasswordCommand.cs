using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

public class ResetPasswordCommand : IRequest<ApiResponse<bool>>
{
    public string Token { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}