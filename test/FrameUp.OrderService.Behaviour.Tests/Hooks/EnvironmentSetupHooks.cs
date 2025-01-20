using BoDi;
using FrameUp.OrderService.Behaviour.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Hooks;

[Binding]
public class EnvironmentSetupHooks
{
    private static PostgresqlFixture postgresql;

    [BeforeTestRun]
    public static async Task BeforeTestRun(IObjectContainer testThreadContainer)
    {
        var httpClient = HttpClientFactory.Create();

        var application = new WebApplicationFactory<Program>();

        var orderServiceClientApi = new OrderServiceClientApi("", application.CreateClient());

        testThreadContainer.RegisterInstanceAs(orderServiceClientApi);

        postgresql = new PostgresqlFixture();
        await postgresql.BaseSetUp();
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await postgresql.BaseTearDown();
    }
}
