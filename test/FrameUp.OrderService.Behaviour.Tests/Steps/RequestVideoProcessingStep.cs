using FluentAssertions;
using FrameUp.OrderService.Behaviour.Tests.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Steps;

[Binding]
public class RequestVideoProcessingStep(
    OrderServiceClientApi orderServiceClientApi,
    ScenarioContext scenarioContext
    ) : TestHelpers
{
    [BeforeScenario]
    public async Task Setup()
    {
    }
    
    [Given(@"I have a video file")]
    public void GivenIHaveAVideoFileAsync()
    {
        var videos = new List<FileParameter>
        {
            new(CreateFakeVideo(), "video.mp4", "video/mp4")
        };

        scenarioContext["videos"] = videos;
    }

    [Given(@"Define Parameters")]
    public void GivenDefineParameters()
    {
        var resolution = ResolutionTypes.FullHD;
        var captureInterval = 10;

        scenarioContext["resolution"] = resolution;
        scenarioContext["captureInterval"] = captureInterval;
    }

    [When(@"I request the video to be processed")]
    public async Task WhenIRequestTheVideoToBeProcessedAsync()
    {
        var resolution = scenarioContext.Get<ResolutionTypes>("resolution");

        var captureInterval = scenarioContext.Get<int>("captureInterval");

        var videos = scenarioContext.Get<List<FileParameter>>("videos");

        var response = await orderServiceClientApi.OrderPOSTAsync(captureInterval, resolution, videos);

        scenarioContext["response"] = response;
    }

    [Then(@"the video should start processing")]
    public void ThenTheVideoShouldStartProcessing()
    {
        var response = scenarioContext.Get<CreateProcessingOrderResponse>("response");

        response.IsValid.Should().BeTrue();

        response.Status.Should().Be(ProcessingStatus.Processing);

    }
}