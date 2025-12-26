using MediatR;
using MyApi.Application.Abstractions;
using MyApi.Application.Dtos;

namespace MyApi.Application.Todos.Queries.GetTodos;

public class GetTodosHandler : IRequestHandler<GetTodosQuery, List<TodoReadDto>>
{
    private readonly ITodoRepository _repo;
    public GetTodosHandler(ITodoRepository repo) => _repo = repo;

    public async Task<List<TodoReadDto>> Handle(GetTodosQuery request, CancellationToken ct)
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
}
