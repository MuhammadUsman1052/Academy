
using MediatR;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Application.Abstractions;
public record GetUsersQuery() : IRequest<ApiResponse<List<User>>>;
