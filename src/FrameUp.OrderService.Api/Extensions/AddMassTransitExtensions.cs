using FrameUp.OrderService.Api.Configuration;
using FrameUp.OrderService.Application.Consumers;
using MassTransit;

namespace FrameUp.OrderService.Api.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection serviceCollection, Settings settings)
    {
        serviceCollection.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("frameup-order-service"));

            busConfigurator.AddConsumer<ProcessVideoConsumer>();

            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(settings.RabbitMQ.ConnectionString));

                configurator.AutoDelete = true;
                configurator.ConfigureEndpoints(context);
            });

        });

        return serviceCollection;
    }
}