using FluentAssertions;
using FrameUp.OrderService.Api.Models;
using FrameUp.OrderService.Behaviour.Tests.Fixtures;
using FrameUp.OrderService.Behaviour.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        var resolution = 1080;

        scenarioContext["resolution"] = resolution;
    }

    [When(@"I request the video to be processed")]
    public async Task WhenIRequestTheVideoToBeProcessedAsync()
    {
        var resolution = scenarioContext.Get<int>("resolution");
        var videos = scenarioContext.Get<List<FileParameter>>("videos");

        var response = await orderServiceClientApi.OrderAsync(resolution, videos);

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