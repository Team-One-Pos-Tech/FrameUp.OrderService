using FrameUp.OrderService.Domain.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;

namespace FrameUp.OrderService.Behaviour.Tests.Extensions;

public static class MockRegister
{
    public static IWebHostBuilder UseMocks(this IWebHostBuilder builder)
    {
        var mock = new Mock<IPublishEndpoint>();
        var authenticationMock = new Mock<IAuthenticatedUser>();

        authenticationMock.Setup(x => x.UserId)
            .Returns(Guid.NewGuid());

        builder.ConfigureServices(services =>
            services
                .AddScoped<IPublishEndpoint>(_ => mock.Object)
                .AddScoped<IAuthenticatedUser>(_ => authenticationMock.Object)
        );

        return builder;
    }
}