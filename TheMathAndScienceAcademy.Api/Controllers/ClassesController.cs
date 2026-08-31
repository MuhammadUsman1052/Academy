using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.Classes.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Classes.Query.Models;

namespace TheMathAndScienceAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/classes")]
public class ClassesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ClassesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [HasPermission("class.create")]
    public async Task<IActionResult> Create([FromBody] CreateAcademicClassCommand command) => Ok(await _mediator.Send(command));

    [HttpGet]
    [HasPermission("class.view")]
    public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAcademicClassesQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("class.view")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _mediator.Send(new GetAcademicClassByIdQuery { Id = id }));

    [HttpPut("{id:guid}")]
    [HasPermission("class.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAcademicClassCommand command) { command.Id = id; return Ok(await _mediator.Send(command)); }

    [HttpDelete("{id:guid}")]
    [HasPermission("class.delete")]
    public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteAcademicClassCommand { Id = id }));
}
