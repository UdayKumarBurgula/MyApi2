



Clean Architecture structure for your .NET 8 Web API + EF Core + Postgres project, without overcomplicating it.
---------------------------------------------------------------------------------------------------------------

dotnet new sln -n MyApi

mkdir src
cd src

dotnet new webapi -n MyApi.Api
dotnet new classlib -n MyApi.Application
dotnet new classlib -n MyApi.Domain
dotnet new classlib -n MyApi.Infrastructure

cd ..
dotnet sln add src/MyApi.Api/MyApi.Api.csproj
dotnet sln add src/MyApi.Application/MyApi.Application.csproj
dotnet sln add src/MyApi.Domain/MyApi.Domain.csproj
dotnet sln add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj


dotnet add src/MyApi.Api/MyApi.Api.csproj reference src/MyApi.Application/MyApi.Application.csproj
dotnet add src/MyApi.Application/MyApi.Application.csproj reference src/MyApi.Domain/MyApi.Domain.csproj
dotnet add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj reference src/MyApi.Application/MyApi.Application.csproj
dotnet add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj reference src/MyApi.Domain/MyApi.Domain.csproj

dotnet add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj package Microsoft.EntityFrameworkCore --version 8.0.8
dotnet add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.8
dotnet add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4
dotnet add src/MyApi.Infrastructure/MyApi.Infrastructure.csproj package EFCore.NamingConventions --version 8.0.3

use TargetFramework net8.0
-------------------------------------
net8.0

MyApi2>dotnet tool run dotnet-ef migrations add InitialCreate2 --project src/MyApi.Infrastructure/MyApi.Infrastructure.csproj  --startup-project src/MyApi.Api/MyApi.Api.csproj  --context AppDbContext --output-dir Persistence/Migrations
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'

MyApi2>dotnet tool run dotnet-ef database update --project src/MyApi.Infrastructure/MyApi.Infrastructure.csproj --startup-project src/MyApi.Api/MyApi.Api.csproj --context AppDbContext


Clean, practical way to add CQRS (MediatR) + FluentValidation (Application layer) + global exception middleware + Repository + Unit of Work to the Clean Architecture setup we discussed (.NET 8).
-----------------------------
Add CQRS with MediatR
Add FluentValidation in Application layer
Add global exception handling middleware
Add Repository + Unit of Work pattern (if needed)

Add NuGet packages
----------------------
Application project
----------------------
dotnet add src/MyApi.Application/MyApi.Application.csproj package MediatR --version 12.4.1
dotnet add src/MyApi.Application/MyApi.Application.csproj package FluentValidation --version 11.10.0
dotnet add src/MyApi.Application/MyApi.Application.csproj package FluentValidation.DependencyInjectionExtensions --version 11.10.0

API project
--------------
dotnet add src/MyApi.Api/MyApi.Api.csproj package MediatR.Extensions.Microsoft.DependencyInjection --version 12.1.1

Infrastructure stays the same: EF Core + Npgsql packages

Repository + Unit of Work (Infrastructure)


Application layer must NOT reference Infrastructure
---------------------------------------------------------------------
Api → Application → Domain
Infrastructure → Application + Domain


Why MediatR Handlers Are Transient - Why not MediatR be implemented as Singleton?
-----------------------------------------------------------------------------------------

public class CreateTodoHandler
{
    private readonly AppDbContext _db; // ❌ scoped - Cannot consume scoped service from singleton

    public CreateTodoHandler(AppDbContext db) { }
}

Because AppDbContext is registered as scoped, and MediatR handlers are registered as transient by default. 
If the handler were singleton, it would lead to capturing a scoped service in a singleton, 
which is not allowed in ASP.NET Core's DI system. 
This ensures that each request gets its own instance of the handler, which in turn gets its own instance of the scoped services it depends on.

Quick “lifetime safety” rules that prevent most bugs
-----------------------------------------------------------
Singleton must not depend on Scoped. Ever.

Don’t store request/user data in singletons.

No fire-and-forget with scoped services.

DbContext should be Scoped.

Middleware: scoped deps only in InvokeAsync.

How scope behaves with background jobs (needs IServiceScopeFactory)
Background Jobs with Scoped Services in .NET - Best practices
-------------------------------------------------------------------------
Create a new scope per job run (or per message)

Keep background job methods short

Don’t reuse DbContext across iterations

Use try/catch so one failure doesn’t kill the service

Prefer a queue (Channels) if you need to process events


Use separate DbContext instances for parallel EF
-------------------------------------------------------
If you truly need parallel DB calls, create a new DbContext per task.
Best way in ASP.NET Core: IDbContextFactory<TContext>.

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connString));

var t1 = Task.Run(async () =>
{
    await using var db = await _factory.CreateDbContextAsync(ct);
    return await db.TodoItems.AsNoTracking().ToListAsync(ct);
});

var t2 = Task.Run(async () =>
{
    await using var db = await _factory.CreateDbContextAsync(ct);
    return await db.Users.AsNoTracking().ToListAsync(ct);
});

await Task.WhenAll(t1, t2);

One context, one transaction, no parallel EF
-----------------------------------------------------
If you need atomic work across multiple operations, keep one DbContext and do it sequentially within a transaction:

await using var tx = await _db.Database.BeginTransactionAsync(ct);

var a = await _db.TableA.ToListAsync(ct);
var b = await _db.TableB.ToListAsync(ct);

await _db.SaveChangesAsync(ct);
await tx.CommitAsync(ct);

This ensures all operations are part of the same transaction and DbContext instance, avoiding concurrency issues.

