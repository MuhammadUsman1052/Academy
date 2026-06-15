using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Query.Models;

public class GetCurrentUserQuery : IRequest<ApiResponse<CurrentUserDto>>
{
}
