using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.Students.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Students.Query.Models;
using TheMathAndScienceAcademy.Application.Features.Students;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [HasPermission("student.create")]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command) => Ok(await _mediator.Send(command));

    [HttpPut("{id:guid}")]
    [HasPermission("student.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("student.delete")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteStudentCommand { Id = id }));

    [HttpGet]
    [HasPermission("student.view")]
    public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetStudentsQuery()));

    [HttpGet("me")]
    [HasPermission("student.self-view")]
    public async Task<IActionResult> GetMyProfile() => Ok(await _mediator.Send(new GetMyStudentProfileQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("student.view")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _mediator.Send(new GetStudentByIdQuery { Id = id }));

    [HttpGet("by-academy/{academyId:guid}")]
    [HasPermission("student.view-by-academy")]
    public async Task<IActionResult> GetByAcademy(Guid academyId) => Ok(await _mediator.Send(new GetStudentsByAcademyQuery { AcademyId = academyId }));

    [HttpPost("{id:guid}/resend-credentials")]
    [HasPermission("student.resend-credentials")]
    public async Task<IActionResult> ResendCredentials(Guid id) => Ok(await _mediator.Send(new ResendStudentCredentialsCommand { StudentId = id }));

    [HttpPost("{id:guid}/create-account")]
    [HasPermission("student.create-account")]
    public async Task<IActionResult> CreateAccount(Guid id) => Ok(await _mediator.Send(new CreateStudentAccountCommand { StudentId = id }));

    [HttpPost("{id:guid}/photo")]
    [Consumes("multipart/form-data")]
    [HasPermission("student.update")]
    public async Task<IActionResult> UploadPhoto(Guid id, IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        return Ok(await _mediator.Send(new UploadStudentPhotoCommand(id, new ProfilePhotoUpload(stream, file.ContentType, file.Length))));
    }

    [HttpGet("{id:guid}/photo")]
    [HasPermission("student.view")]
    public async Task<IActionResult> GetPhoto(Guid id) => await SendPhoto(new GetStudentPhotoQuery(id));

    [HttpDelete("{id:guid}/photo")]
    [HasPermission("student.update")]
    public async Task<IActionResult> DeletePhoto(Guid id) => Ok(await _mediator.Send(new DeleteStudentPhotoCommand(id)));

    [HttpGet("me/photo")]
    [HasPermission("student.self-view")]
    public async Task<IActionResult> GetMyPhoto() => await SendPhoto(new GetMyStudentPhotoQuery());

    private async Task<IActionResult> SendPhoto(IRequest<ApiResponse<ProfilePhotoContent>> query)
    {
        var result = await _mediator.Send(query);
        return !result.Success || result.Data is null ? NotFound(result) : File(result.Data.Content, result.Data.ContentType);
    }
}
