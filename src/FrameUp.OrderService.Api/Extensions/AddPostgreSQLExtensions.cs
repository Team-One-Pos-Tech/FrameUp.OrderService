using FrameUp.OrderService.Api.Configuration;
using FrameUp.OrderService.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddPostgreSQLExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        var settings = configuration.GetSection("Storage:PostgreSQL").Get<PostgreSQLSettings>()!;
        var connectionString = $"Host={settings.Host};Username={settings.UserName};Password={settings.Password};Database={settings.Database}";

        serviceCollection
            .AddDbContext<OrderDbContext>(options =>
                options.UseNpgsql(connectionString));

        return serviceCollection;
    }
}
