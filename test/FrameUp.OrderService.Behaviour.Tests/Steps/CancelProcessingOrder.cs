using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Steps;

[Binding]
public class CancelProcessingOrder(
    ScenarioContext scenarioContext,
    OrderServiceClientApi orderServiceClientApi
    )
{
    [WhenAttribute(@"I cancel processing order")]
    public async Task WhenICancelProcessingOrder()
    {
        var createdOrder = scenarioContext.Get<CreateProcessingOrderResponse>("createOrderResponse");

        await orderServiceClientApi.CancelAsync(createdOrder.Id);
    }

    [ThenAttribute(@"Order status should be cancelled")]
    public async Task ThenOrderStatusShouldBeCancelled()
    {
        var createdOrder = scenarioContext.Get<CreateProcessingOrderResponse>("createOrderResponse");

        var response = await orderServiceClientApi.OrderGETAsync(createdOrder.Id);
        
        response.Id.Should().Be(createdOrder.Id);
        response.Status.Should().Be(ProcessingStatus.Canceled);
    }
}