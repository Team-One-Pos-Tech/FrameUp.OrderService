using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FrameUp.OrderService.Behaviour.Tests.Extensions;

public static class MockRegister
{
    public static IWebHostBuilder UseMocks(this IWebHostBuilder builder)
    {
        var mock = new Mock<IPublishEndpoint>();

        builder.ConfigureServices(services =>
            services.AddScoped<IPublishEndpoint>(_ => mock.Object)
        );

        return builder;
    }
}