using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Command.Models;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Query.Models;

namespace TheMathAndScienceAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/student-enrollments")]
public class StudentEnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public StudentEnrollmentsController(IMediator mediator) => _mediator = mediator;
    [HttpPost]
    [HasPermission("enrollment.create")]
    public async Task<IActionResult> Create([FromBody] CreateStudentEnrollmentCommand command) => Ok(await _mediator.Send(command));
    [HttpGet]
    [HasPermission("enrollment.view")]
    public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetStudentEnrollmentsQuery()));
    [HttpGet("by-student/{studentId:guid}")]
    [HasPermission("enrollment.view")]
    public async Task<IActionResult> ByStudent(Guid studentId) => Ok(await _mediator.Send(new GetEnrollmentsByStudentQuery { StudentId = studentId }));
    [HttpGet("by-class/{classId:guid}")]
    [HasPermission("enrollment.view")]
    public async Task<IActionResult> ByClass(Guid classId) => Ok(await _mediator.Send(new GetEnrollmentsByClassQuery { ClassId = classId }));
    [HttpPut("{id:guid}")]
    [HasPermission("enrollment.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentEnrollmentCommand command) { command.Id = id; return Ok(await _mediator.Send(command)); }
    [HttpDelete("{id:guid}")]
    [HasPermission("enrollment.delete")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteStudentEnrollmentCommand { Id = id }));
}
