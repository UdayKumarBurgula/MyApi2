using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApi.Application.Abstractions;
using MyApi.Infrastructure.Persistence;

namespace MyApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default"))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<ITodoRepository, TodoRepository>();
        return services;
    }
}
