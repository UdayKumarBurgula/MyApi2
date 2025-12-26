using MediatR;
using MyApi.Application.Abstractions;
using MyApi.Application.Dtos;
using MyApi.Domain.Entities;

namespace MyApi.Application.Todos.Commands.CreateTodo;

public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, TodoReadDto>
{
    private readonly ITodoRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateTodoHandler(ITodoRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<TodoReadDto> Handle(CreateTodoCommand request, CancellationToken ct)
    {
        var entity = new TodoItem
        {
            Title = request.Title.Trim(),
            IsDone = request.IsDone
        };

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return new TodoReadDto
        {
            Id = entity.Id,
            Title = entity.Title,
            IsDone = entity.IsDone,
            CreatedUtc = entity.CreatedUtc
        };
    }
}
