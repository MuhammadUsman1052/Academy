
using MediatR;
using TheMathAndScienceAcademy.Domain.Repositories;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Domain.Entities;
public class GetUsersHandler : IRequestHandler<GetUsersQuery, ApiResponse<List<User>>>
{
    private readonly IUserRepository _repo;
    public GetUsersHandler(IUserRepository repo) => _repo = repo;
    public async Task<ApiResponse<List<User>>> Handle(GetUsersQuery _, CancellationToken __)
        => ApiResponse<List<User>>.Ok(await _repo.GetAllAsync());
}
