using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Users.Command.Models;

public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Guid RoleId { get; set; }
    public Guid? AcademyId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
}
