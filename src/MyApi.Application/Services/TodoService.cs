using MyApi.Application.Abstractions;
using MyApi.Application.Dtos;
using MyApi.Domain.Entities;

namespace MyApi.Application.Services;

public class TodoService
{
    private readonly ITodoRepository _repo;
    public TodoService(ITodoRepository repo) => _repo = repo;

    public async Task<List<TodoReadDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(x => new TodoReadDto
        {
            Id = x.Id,
            Title = x.Title,
            IsDone = x.IsDone,
            CreatedUtc = x.CreatedUtc
        }).ToList();
    }

    public async Task<TodoReadDto> CreateAsync(TodoCreateDto dto, CancellationToken ct = default)
    {
        var entity = new TodoItem { Title = dto.Title.Trim(), IsDone = dto.IsDone };
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return new TodoReadDto
        {
            Id = entity.Id,
            Title = entity.Title,
            IsDone = entity.IsDone,
            CreatedUtc = entity.CreatedUtc
        };
    }
}
