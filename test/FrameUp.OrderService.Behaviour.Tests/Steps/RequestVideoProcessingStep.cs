using FrameUp.OrderService.Api.Models;
using FrameUp.OrderService.Behaviour.Tests.Fixtures;
using FrameUp.OrderService.Behaviour.Tests.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Steps;

[Binding]
public class RequestVideoProcessingStep(OrderServiceClientApi orderServiceClientApi): TestHelpers
{
    [BeforeScenario]
    public async Task Setup()
    {
    }
    
    [Given(@"I have a video file")]
    public async Task GivenIHaveAVideoFileAsync()
    {
        var resolution = 1080;

        var files = new List<FileParameter>
        {
            new(CreateFakeVideo())
        };

        await orderServiceClientApi.OrderAsync(resolution, files);
    }

    [Given(@"Define Parameters")]
    public void GivenDefineParameters()
    {
        ScenarioContext.StepIsPending();
    }

    [When(@"I request the video to be processed")]
    public void WhenIRequestTheVideoToBeProcessed()
    {
        ScenarioContext.StepIsPending();
    }

    [Then(@"the video should start processing")]
    public void ThenTheVideoShouldStartProcessing()
    {
        ScenarioContext.StepIsPending();
    }
}