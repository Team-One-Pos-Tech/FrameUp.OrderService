using FrameUp.OrderService.Api.Services;
using FrameUp.OrderService.Domain.Contracts;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IAuthenticatedUser, AuthenticatedUser>();

        return serviceCollection;
    }
}