
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Contracts;
using FrameUp.OrderService.Infra.Repositories;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddRepositoriesExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IFileBucketRepository, FileBucketRepository>()
            .AddScoped<ILocalStoreRepository, LocalStoreRepository>()
            .AddScoped<IOrderRepository, OrderRepository>();
        
        return serviceCollection;
    }
}