using FrameUp.OrderService.Api.Services;
using FrameUp.OrderService.Application.Services;
using FrameUp.OrderService.Domain.Contracts;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddHostedService<UploadVideosService>()
            .AddScoped<IAuthenticatedUser, AuthenticatedUser>();

        return serviceCollection;
    }
}