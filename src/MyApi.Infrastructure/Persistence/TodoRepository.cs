using Microsoft.EntityFrameworkCore;
using MyApi.Application.Abstractions;
using MyApi.Domain.Entities;

namespace MyApi.Infrastructure.Persistence;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _db;
    public TodoRepository(AppDbContext db) => _db = db;

    public Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default) =>
        _db.TodoItems.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(ct);

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.TodoItems.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct = default)
    {
        await _db.TodoItems.AddAsync(item, ct);
        return item;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.TodoItems.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;
        _db.TodoItems.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
