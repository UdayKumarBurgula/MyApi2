using MyApi.Domain.Entities;

namespace MyApi.Application.Abstractions;

public interface ITodoRepository
{
    Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
