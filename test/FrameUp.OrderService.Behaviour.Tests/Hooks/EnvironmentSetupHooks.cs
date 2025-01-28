using BoDi;
using FrameUp.OrderService.Behaviour.Tests.Fixtures;
using FrameUp.OrderService.Infra.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using FrameUp.OrderService.Api;
using FrameUp.OrderService.Behaviour.Tests.Extensions;
using TechTalk.SpecFlow;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                        .UseNpgsql(postgresql.ConnectionString)
                        .Options;

                    var productionDbContext = new OrderServiceDbContext(dbContextOptions);
                    collection.AddSingleton<OrderServiceDbContext>(_ => productionDbContext);
                });
            });

        var httpClient = application.CreateClient();

        var jwtMockToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MzgxMDI3MTMsImlzcyI6IlJhbmRvbUlzc3VlciIsImF1ZCI6IlJhbmRvbUF1ZGllbmNlIn0.XyKa5pWMzBwHy7Q08mwFpG1mEHPcSsFSUNHpog4tYAg";

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtMockToken}");

        var orderServiceClientApi = new OrderServiceClientApi("", httpClient);

        testThreadContainer.RegisterInstanceAs(orderServiceClientApi);

    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await postgresql.BaseTearDown();
    }
}
