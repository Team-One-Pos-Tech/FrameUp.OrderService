using FluentAssertions;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.UseCases;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace FrameUp.OrderService.Application.Tests.UseCases;

[TestFixture]
[TestOf(typeof(GetProcessingOrder))]
public class GetProcessingOrderShould
{
    
    private GetProcessingOrder _getProcessingOrder;
    private Mock<IOrderRepository> _orderRepository;
    private Mock<ILogger<GetProcessingOrder>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<GetProcessingOrder>>();

        _getProcessingOrder = new GetProcessingOrder(
            _loggerMock.Object,
            _orderRepository.Object);
    }

    [Test]
    public async Task Get_Order_By_Id()
    {
        #region Arrange

        var request = new GetProcessingOrderRequest
        {
            OrderId = Guid.NewGuid()
        };
        
        _orderRepository.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order()
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing, // Adjust based on expected status
                OwnerId = Guid.Empty, // Adjust based on expected owner ID
                Videos = new List<VideoMetadata>
                {
                    new VideoMetadata
                    {
                        Id = Guid.NewGuid(),
                        Name = "video.mp4",
                        ContentType = "video/mp4",
                        Size = 1024
                    }
                },
                ExportResolution = ResolutionTypes.FullHD, // Adjust based on expected resolution
                CaptureInterval = null // Adjust based on expected capture interval
            });
        
        #endregion

        #region Act

        var response = await _getProcessingOrder.GetById(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        
        response!.Id.Should().Be(request.OrderId);
        response.Status.Should().Be(ProcessingStatus.Processing); // Adjust based on expected status
        response.OwnerId.Should().Be(Guid.Empty); // Adjust based on expected owner ID
        response.Videos.Should().NotBeEmpty();
        response.ExportResolution.Should().Be(ResolutionTypes.FullHD); // Adjust based on expected resolution
        response.CaptureInterval.Should().BeNull(); // Adjust based on expected capture interval
        
        #endregion
    }
}