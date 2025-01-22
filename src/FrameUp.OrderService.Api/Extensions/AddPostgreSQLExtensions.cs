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

        serviceCollection
            .AddDbContext<OrderServiceDbContext>(options =>
                options.UseNpgsql(settings.ConnectionString));

        return serviceCollection;
    }
}
