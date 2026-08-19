using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.Users.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Users.Query.Models;

namespace TheMathAndScienceAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission("user.create")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("{id:guid}")]
    [HasPermission("user.update")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("user.delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
        => Ok(await _mediator.Send(new DeleteUserCommand { Id = id }));

    [HttpGet]
    [HasPermission("user.view")]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetUsersQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("user.view")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
        => Ok(await _mediator.Send(new GetUserByIdQuery { Id = id }));

    [HttpGet("by-academy/{academyId:guid}")]
    [HasPermission("user.view")]
    public async Task<IActionResult> GetByAcademy([FromRoute] Guid academyId)
        => Ok(await _mediator.Send(new GetUsersByAcademyQuery { AcademyId = academyId }));
}
