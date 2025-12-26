using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApi.Application.Abstractions;

namespace MyApi.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _uow;

    public TransactionBehavior(IUnitOfWork uow) => _uow = uow;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Only wrap "commands" in a transaction (optional convention)
        var isCommand = request.GetType().Name.EndsWith("Command", StringComparison.Ordinal);
        if (!isCommand)
            return await next();

        await using var tx = await _uow.BeginTransactionAsync(ct);

        try
        {
            var response = await next();

            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return response;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
