using FrameUp.OrderService.Api.Configuration;
using Minio;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddMinIOExtensions
{

    public static IServiceCollection AddMinIO(this IServiceCollection serviceCollection, Settings settings)
    {
        serviceCollection.AddMinio(configureClient => configureClient
            .WithEndpoint(settings.MinIO.Endpoint)
            .WithCredentials(settings.MinIO.AccessKey, settings.MinIO.SecretKey)
            .Build());

        return serviceCollection;
    }
}