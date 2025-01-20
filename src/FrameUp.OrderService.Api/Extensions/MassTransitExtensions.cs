using FrameUp.OrderService.Api.Configuration;
using FrameUp.OrderService.Api.Extensions;
using MassTransit;

namespace FrameUp.OrderService.Api.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()!;

        serviceCollection.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("order-service"));
            //busConfigurator.AddConsumer<T>();

            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(settings.Host, "/", rabbitMqHostConfigurator =>
                {
                    rabbitMqHostConfigurator.Username(settings.UserName);
                    rabbitMqHostConfigurator.Password(settings.Password);
                });

                configurator.AutoDelete = true;
                configurator.ConfigureEndpoints(context);
            });

        });

        return serviceCollection;
    }
}