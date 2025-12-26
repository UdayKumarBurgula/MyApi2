using Microsoft.EntityFrameworkCore;
using MyApi.Infrastructure.Persistence;

namespace MyApi.Api.Background;

public class TodoCleanupJob
{
    private readonly AppDbContext _db;

    public TodoCleanupJob(AppDbContext db)
    {
        _db = db;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        // Example: delete completed todos older than 30 days
        var cutoff = DateTime.UtcNow.AddDays(-30);

        var oldDone = await _db.TodoItems
            .Where(x => x.IsDone && x.CreatedUtc < cutoff)
            .ToListAsync(ct);

        if (oldDone.Count == 0) return;

        _db.TodoItems.RemoveRange(oldDone);
        await _db.SaveChangesAsync(ct);
    }
}
