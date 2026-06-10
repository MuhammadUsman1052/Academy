
using MediatR;
using TheMathAndScienceAcademy.Domain.Repositories;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Application.Abstractions;
public class CreateUserHandler : IRequestHandler<CreateUserCommand, ApiResponse<string>>
{
    private readonly IUserRepository _repo;
    public CreateUserHandler(IUserRepository repo) => _repo = repo;
    public async Task<ApiResponse<string>> Handle(CreateUserCommand r, CancellationToken _)
    {
        var user = new User { Name = r.Name, Email = r.Email };
        await _repo.CreateAsync(user);
        return ApiResponse<string>.Ok(user.Id);
    }
}
