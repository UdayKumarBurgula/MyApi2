using MediatR;
using MyApi.Application.Dtos;

namespace MyApi.Application.Todos.Commands.CreateTodo;

public record CreateTodoCommand(string Title, bool IsDone) : IRequest<TodoReadDto>;
