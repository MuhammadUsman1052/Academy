using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Permissions.Query.Models;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission("permission.create")]
    public async Task<IActionResult> Create([FromBody] CreatePermissionCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("{id:guid}")]
    [HasPermission("permission.update")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePermissionCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("permission.delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
        => Ok(await _mediator.Send(new DeletePermissionCommand { Id = id }));

    [HttpGet]
    [HasPermission("permission.view")]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetPermissionsQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("permission.view")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
        => Ok(await _mediator.Send(new GetPermissionByIdQuery { Id = id }));
}
