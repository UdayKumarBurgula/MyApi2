using MyApi.Domain.Entities;

namespace MyApi.Application.Abstractions;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(TodoItem item, CancellationToken ct = default);
    void Remove(TodoItem item);
}
