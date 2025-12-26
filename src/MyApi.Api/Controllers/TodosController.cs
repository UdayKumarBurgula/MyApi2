using Microsoft.AspNetCore.Mvc;
using MyApi.Application.Dtos;
using MyApi.Application.Services;

namespace MyApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly TodoService _svc;
    public TodosController(TodoService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult> GetAll(CancellationToken ct) =>
        Ok(await _svc.GetAllAsync(ct));

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TodoCreateDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(dto, ct);
        return Created($"/api/todos/{created.Id}", created);
    }
}
