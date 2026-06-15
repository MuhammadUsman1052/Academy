using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Command.Models;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Query.Models;

[ApiController]
[Route("api/role-permissions")]
public class RolePermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolePermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign")]
    [HasPermission("rolepermission.assign")]
    public async Task<IActionResult> Assign([FromBody] AssignPermissionToRoleCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("remove")]
    [HasPermission("rolepermission.remove")]
    public async Task<IActionResult> Remove([FromBody] RemovePermissionFromRoleCommand command)
        => Ok(await _mediator.Send(command));

    [HttpGet("{roleId:guid}")]
    [HasPermission("rolepermission.view")]
    public async Task<IActionResult> GetByRoleId([FromRoute] Guid roleId)
        => Ok(await _mediator.Send(new GetRolePermissionsQuery { RoleId = roleId }));
}
