using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Roles.Query.Models;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission("role.create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("{id:guid}")]
    [HasPermission("role.update")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRoleCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("role.delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
        => Ok(await _mediator.Send(new DeleteRoleCommand { Id = id }));

    [HttpGet]
    [HasPermission("role.view")]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetRolesQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("role.view")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
        => Ok(await _mediator.Send(new GetRoleByIdQuery { Id = id }));
}
