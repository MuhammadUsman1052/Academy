
using Microsoft.AspNetCore.Mvc;
using MediatR;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand cmd)
        => Ok(await _mediator.Send(cmd));

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _mediator.Send(new GetUsersQuery()));
}
