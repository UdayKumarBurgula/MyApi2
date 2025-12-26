using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using MyApi.Application.Abstractions;

namespace MyApi.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly ILogger<UnitOfWork> _logger;
    public UnitOfWork(AppDbContext db) => _db = db;

    public UnitOfWork(AppDbContext db, ILogger<UnitOfWork> logger) 
    {
        _db = db;
        _logger = logger;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("UoW DbContext InstanceId: {Id}", _db.InstanceId);
        return _db.SaveChangesAsync(ct);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default) =>
        _db.Database.BeginTransactionAsync(ct);

}
