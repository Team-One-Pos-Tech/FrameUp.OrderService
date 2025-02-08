using System;
using FluentAssertions;
using FrameUp.OrderService.Behaviour.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameUp.OrderService.Application.Models.Events;
using TechTalk.SpecFlow;

namespace FrameUp.OrderService.Behaviour.Tests.Steps;

[Binding]
public class GetProcessingOrdersSteps(
    OrderServiceClientApi orderServiceClientApi,
    ScenarioContext scenarioContext
    ) : TestHelpers
{

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

    [GivenAttribute(@"There is concluded processing orders")]
    public async Task GivenThereIsConcludedProcessingOrders()
    {
        var videoName = "video.mp4";

        var videos = new List<FileParameter>
        {
            new(CreateFakeVideo(), videoName, "video/mp4")
        };

        var response = await orderServiceClientApi.OrderPOSTAsync(10, ResolutionTypes.FullHD, videos);

        var uploadedFile = new UpdatePackageItemRequest()
        {
            FileName = "file1.mp4", Uri = "https://s3.com/file1"
        };
        
        var request = new UpdateProcessingOrderRequest {
            OrderId = response.Id,
            Status = ProcessingStatus.Concluded,
            Packages = [uploadedFile]
        };

        await orderServiceClientApi.OrderPUTAsync(request);
        
        scenarioContext["concludedOrderId"] = response.Id;
        scenarioContext["uploadedFile"] = uploadedFile;
    }

    [ThenAttribute(@"I have concluded processing orders with pacakges")]
    public async Task ThenIHaveConcludedProcessingOrdersWithPacakges()
    {
        var concludedOrderId = scenarioContext.Get<Guid>("concludedOrderId");
        
        var order = await orderServiceClientApi.OrderGETAsync(concludedOrderId);

        order.Status.Should().Be(ProcessingStatus.Concluded);
        
        order.Packages.Count.Should().Be(1);
        
        var uploadedFile = scenarioContext.Get<UpdatePackageItemRequest>("uploadedFile");
        
        var package = order.Packages.First();
        
        package.FileName.Should().Be(uploadedFile.FileName);
        
        package.Uri.Should().Be(uploadedFile.Uri);
    }
}
