using FrameUp.OrderService.Api.Configuration;
using FrameUp.OrderService.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddPostgreSQLExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection serviceCollection,
        Settings settings
    )
    {
        serviceCollection
            .AddDbContext<OrderServiceDbContext>(options =>
                options.UseNpgsql(settings.PostgreSQL.ConnectionString));

        return serviceCollection;
    }
}
