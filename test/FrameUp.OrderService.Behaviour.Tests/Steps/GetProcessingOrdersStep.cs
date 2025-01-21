using FrameUp.OrderService.Behaviour.Tests.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Steps;

[Binding]
public class GetProcessingOrdersSteps(
    OrderServiceClientApi orderServiceClientApi,
    ScenarioContext scenarioContext
    ) : TestHelpers
{
    [Given("I am logged in")]
    public void GivenIAmLoggedIn()
    {
        // Implement login logic here
    }

    [Given("There is processing orders")]
    public async Task GivenThereIsProcessingOrdersAsync()
    {
        var videoName = "video.mp4";

        var videos = new List<FileParameter>
        {
            new(CreateFakeVideo(), videoName, "video/mp4")
        };

        var response = await orderServiceClientApi.OrderPOSTAsync(null, null, videos);

        scenarioContext["response"] = response;
    }

    [When("I get processing orders")]
    public void WhenIGetProcessingOrders()
    {

        var response = await orderServiceClientApi.OrderGETAsync();

    }

    [Then("I should see a list of my processing orders")]
    public void ThenIShouldSeeAListOfMyProcessingOrders()
    {
        // Implement logic to verify the list of processing orders
    }
}
