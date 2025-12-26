using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApi.Application.Abstractions;
using MyApi.Domain.Entities;

namespace MyApi.Infrastructure.Persistence;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<TodoRepository> _logger;

    public TodoRepository(AppDbContext db, ILogger<TodoRepository> logger) 
    { 
        _db = db; 
        _logger = logger; 
    }

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default) =>
    _db.TodoItems.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default) =>
        _db.TodoItems.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(ct);

    public Task AddAsync(TodoItem item, CancellationToken ct = default)
    {
        _logger.LogInformation("Repo DbContext InstanceId: {Id}", _db.InstanceId);
        return _db.TodoItems.AddAsync(item, ct).AsTask();
    }

    public void Remove(TodoItem item) => _db.TodoItems.Remove(item);
}
