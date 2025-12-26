using MediatR;
using Microsoft.Extensions.Logging;
using MyApi.Application.Abstractions;
using MyApi.Application.Dtos;
using MyApi.Domain.Entities;

using System;

namespace MyApi.Application.Todos.Commands.CreateTodo;

public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, TodoReadDto>
{
    private readonly ITodoRepository _repo;
    private readonly IUnitOfWork _uow;
    // private readonly AppDbContext _db; // optional: inject directly to prove it's same (X)
    private readonly ILogger<CreateTodoHandler> _logger;

    public CreateTodoHandler(ITodoRepository repo, IUnitOfWork uow, 
        ILogger<CreateTodoHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<TodoReadDto> Handle(CreateTodoCommand request, CancellationToken ct)
    {
        // _logger.LogInformation("Handler DbContext InstanceId: {Id}", _db.InstanceId); (x)
        // Logging can be done via repo/uow if needed

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
