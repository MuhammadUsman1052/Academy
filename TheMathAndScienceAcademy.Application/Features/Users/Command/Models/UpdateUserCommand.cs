using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Users.Command.Models;

public class UpdateUserCommand : IRequest<ApiResponse<UserDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Password { get; set; }
    public Guid RoleId { get; set; }
    public Guid? AcademyId { get; set; }
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
}
