using Microsoft.EntityFrameworkCore;
using MyApi.Domain.Entities;

namespace MyApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
        });
    }
}
