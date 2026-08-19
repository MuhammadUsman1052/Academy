using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Users.Query.Models;

public class GetUserByIdQuery : IRequest<ApiResponse<UserDto>>
{
    public Guid Id { get; set; }
}
