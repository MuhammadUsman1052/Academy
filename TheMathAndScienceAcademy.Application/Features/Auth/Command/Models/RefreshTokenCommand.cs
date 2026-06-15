using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

public class RefreshTokenCommand : IRequest<ApiResponse<LoginResponseDto>>
{
    public string RefreshToken { get; set; } = default!;
}