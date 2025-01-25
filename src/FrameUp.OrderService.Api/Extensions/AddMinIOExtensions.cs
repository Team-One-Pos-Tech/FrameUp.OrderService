using FrameUp.OrderService.Api.Configuration;
using Minio;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddMinIOExtensions
{

    public static IServiceCollection AddMinIO(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
        var settings = configuration.GetSection("Storage:MinIO").Get<MinIOSettings>()!;

        serviceCollection.AddMinio(configureClient => configureClient
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(settings.AccessKey, settings.SecretKey)
            .Build());

        return serviceCollection;
    }
}