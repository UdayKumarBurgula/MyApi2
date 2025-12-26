using MediatR;
using MyApi.Application.Dtos;

namespace MyApi.Application.Todos.Queries.GetTodos;

public record GetTodosQuery() : IRequest<List<TodoReadDto>>;
