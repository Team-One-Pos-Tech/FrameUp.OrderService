using BoDi;
using FrameUp.OrderService.Behaviour.Tests.Fixtures;
using FrameUp.OrderService.Infra.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using FrameUp.OrderService.Behaviour.Tests.Extensions;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Hooks;

[Binding]
public class EnvironmentSetupHooks
{
    private static PostgresqlFixture postgresql;

    [BeforeTestRun]
    public static async Task BeforeTestRun(IObjectContainer testThreadContainer)
    {
        postgresql = new PostgresqlFixture();

        await postgresql.BaseSetUp();
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseMocks();
                builder.ConfigureServices(collection =>
                {
                    collection.RemoveAll<OrderServiceDbContext>();

                    var dbContextOptions = new DbContextOptionsBuilder<OrderServiceDbContext>()
                        .UseNpgsql(postgresql.ConnectionSring)
                        .Options;

                    var productionDbContext = new OrderServiceDbContext(dbContextOptions);
                    collection.AddSingleton<OrderServiceDbContext>(_ => productionDbContext);
                });
            });

        var orderServiceClientApi = new OrderServiceClientApi("", application.CreateClient());

        testThreadContainer.RegisterInstanceAs(orderServiceClientApi);

    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await postgresql.BaseTearDown();
    }
}
