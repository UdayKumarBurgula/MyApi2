using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApi.Application.Todos.Commands.CreateTodo;
using MyApi.Application.Todos.Queries.GetTodos;

namespace MyApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;
    public TodosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetTodosQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoCommand command, CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return Created($"/api/todos/{created.Id}", created);
    }
}
