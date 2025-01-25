

using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.UseCases;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddUseCasesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IGetProcessingOrder, GetProcessingOrder>()
            .AddScoped<IUpdateProcessingOrder, UpdateProcessingOrder>()
            .AddScoped<ICreateProcessingOrder, CreateProcessingOrder>();

        return serviceCollection;
    }
}