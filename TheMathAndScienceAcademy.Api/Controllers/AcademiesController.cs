using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Academies.Query.Models;

[ApiController]
[Route("api/academies")]
public class AcademiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcademiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission("academy.create")]
    public async Task<IActionResult> Create([FromBody] CreateAcademyCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("{id:guid}")]
    [HasPermission("academy.update")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAcademyCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("academy.delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
        => Ok(await _mediator.Send(new DeleteAcademyCommand { Id = id }));

    [HttpGet]
    [HasPermission("academy.view")]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetAcademiesQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("academy.view")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
        => Ok(await _mediator.Send(new GetAcademyByIdQuery { Id = id }));
}
