using FrameUp.OrderService.Behaviour.Tests.Fixtures;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Steps;

[Binding]
public class RequestVideoProcessingStep: PgSQLFixture
{

    [BeforeScenario]
    public async Task Setup()
    {
    }
    
    [Given(@"I have a video file")]
    public static void GivenIHaveAVideoFile()
    {
        ScenarioContext.StepIsPending();
    }

    [Given(@"Define Parameters")]
    public static void GivenDefineParameters()
    {
        ScenarioContext.StepIsPending();
    }

    [When(@"I request the video to be processed")]
    public static void WhenIRequestTheVideoToBeProcessed()
    {
        ScenarioContext.StepIsPending();
    }

    [Then(@"the video should start processing")]
    public static void ThenTheVideoShouldStartProcessing()
    {
        ScenarioContext.StepIsPending();
    }
}