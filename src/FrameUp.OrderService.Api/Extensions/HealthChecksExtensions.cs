﻿using FrameUp.OrderService.Api.Configuration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FrameUp.OrderService.Api.Extensions;
public static class HealthChecksExtensions
{
    public static WebApplicationBuilder AddCustomHealthChecks(this WebApplicationBuilder builder, Settings settings)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(settings.RabbitMQ.ConnectionString),
            AutomaticRecoveryEnabled = true
        };


        builder.Services
            .AddSingleton(factory)
            .AddHealthChecks()
            .AddNpgSql(settings.PostgreSQL.ConnectionString,
               name: "PostgreSQL",
               failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded)
            .AddRabbitMQ(sp => factory.CreateConnectionAsync(), name: "RabbitMQ",
                 failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);



        builder.Services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(5);
            options.MaximumHistoryEntriesPerEndpoint(10);
            options.AddHealthCheckEndpoint("OrderService", "/health");
        }).AddInMemoryStorage();

        return builder;
    }

    public static WebApplication UseCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/dashboard";
        });
        return app;
    }
}
