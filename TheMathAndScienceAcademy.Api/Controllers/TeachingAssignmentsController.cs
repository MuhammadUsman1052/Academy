using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.TeachingAssignments;

namespace TheMathAndScienceAcademy.Api.Controllers;

[ApiController, Authorize, Route("api/teaching-assignments")]
public class TeachingAssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TeachingAssignmentsController(IMediator mediator) => _mediator = mediator;
    [HttpGet, HasPermission("teaching-assignment.view")] public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetTeachingAssignmentsQuery()));
    [HttpGet("me"), HasPermission("teaching-assignment.self-view")] public async Task<IActionResult> Me() => Ok(await _mediator.Send(new GetMyTeachingAssignmentsQuery()));
    [HttpGet("by-teacher/{teacherId:guid}"), HasPermission("teaching-assignment.view")] public async Task<IActionResult> ByTeacher(Guid teacherId) => Ok(await _mediator.Send(new GetTeachingAssignmentsByTeacherQuery(teacherId)));
    [HttpGet("by-class/{classId:guid}"), HasPermission("teaching-assignment.view")] public async Task<IActionResult> ByClass(Guid classId) => Ok(await _mediator.Send(new GetTeachingAssignmentsByClassQuery(classId)));
    [HttpPost, HasPermission("teaching-assignment.create")] public async Task<IActionResult> Create(CreateTeachingAssignmentCommand command) => Ok(await _mediator.Send(command));
    [HttpPut("{id:guid}"), HasPermission("teaching-assignment.update")] public async Task<IActionResult> Update(Guid id, UpdateTeachingAssignmentCommand command) { command.Id = id; return Ok(await _mediator.Send(command)); }
    [HttpDelete("{id:guid}"), HasPermission("teaching-assignment.delete")] public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteTeachingAssignmentCommand(id)));
}
