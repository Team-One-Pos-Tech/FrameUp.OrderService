using FluentAssertions;
using FrameUp.OrderService.Behaviour.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
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

        var response = await orderServiceClientApi.OrderPOSTAsync(10, ResolutionTypes.FullHD, videos);

        scenarioContext["createOrderResponse"] = response;
        scenarioContext["videoName"] = videoName;
    }

    [When("I get processing orders")]
    public async Task WhenIGetProcessingOrdersAsync()
    {
        var listOrders = await orderServiceClientApi.OrderAllAsync();

        scenarioContext["listOrders"] = listOrders;
    }

    [Then("I should see a list of my processing orders")]
    public void ThenIShouldSeeAListOfMyProcessingOrders()
    {
        var listOrders = scenarioContext.Get<IEnumerable<GetProcessingOrderResponse>>("listOrders");
        var videoName = scenarioContext.Get<string>("videoName");

        listOrders!.Count().Should().BeGreaterThanOrEqualTo(1);

        var firstOrder = listOrders.First();

        firstOrder.Videos.Count.Should().BeGreaterThanOrEqualTo(1);
        
        var firstVideo = firstOrder.Videos.First();

        firstVideo.Name.Should().Be(videoName);
    }
}
