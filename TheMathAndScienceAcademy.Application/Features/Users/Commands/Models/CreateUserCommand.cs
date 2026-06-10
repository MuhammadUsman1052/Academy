
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
public record CreateUserCommand(string Name, string Email)
    : IRequest<ApiResponse<string>>;
