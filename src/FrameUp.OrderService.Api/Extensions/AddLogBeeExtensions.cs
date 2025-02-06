using FrameUp.OrderService.Api.Configuration;
using Serilog;
using Serilog.Sinks.LogBee;
using Serilog.Sinks.LogBee.AspNetCore;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddLogBeeExtensions
{
    public static IServiceCollection AddLogBee(this IServiceCollection serviceCollection, Settings settings)
    {
        serviceCollection.AddSerilog((services, lc) => lc
            .WriteTo.LogBee(new LogBeeApiKey(
                    settings.LogBee.OrganizationId,
                    settings.LogBee.ApplicationId,
                    settings.LogBee.ApiUrl
                ),
                services
            )
            .WriteTo.Console());

        return serviceCollection;
    }

    public static void UseLogBee(this WebApplication app)
    {
        app.UseLogBeeMiddleware();
    }
}
