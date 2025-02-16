using FrameUp.OrderService.Api.Services;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddBackgroundServiceExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddHostedService<FilePreprocessorBackgroundService>();

        return serviceCollection;
    }
}