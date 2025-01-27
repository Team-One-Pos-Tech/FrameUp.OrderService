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
                Status = ProcessingStatus.Processing,
                OwnerId = Guid.Empty,
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
                ExportResolution = ResolutionTypes.FullHD,
                CaptureInterval = null
            });
        
        #endregion

        #region Act

        var response = await _getProcessingOrder.GetById(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        
        response!.Id.Should().Be(request.OrderId);
        response.Status.Should().Be(ProcessingStatus.Processing);
        response.OwnerId.Should().Be(Guid.Empty);
        response.Videos.Should().NotBeEmpty();
        response.ExportResolution.Should().Be(ResolutionTypes.FullHD);
        response.CaptureInterval.Should().BeNull();
        
        #endregion
    }
    
    [Test]
    public async Task Get_Order_By_Id_With_Videos()
    {
        #region Arrange

        var request = new GetProcessingOrderRequest
        {
            OrderId = Guid.NewGuid()
        };

        var videoMetadata = new VideoMetadata
        {
            Id = Guid.NewGuid(),
            Name = "video.mp4",
            ContentType = "video/mp4",
            Size = 1024
        };
        
        _orderRepository.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order()
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing,
                OwnerId = Guid.Empty,
                Videos = new List<VideoMetadata>
                {
                    videoMetadata
                },
                ExportResolution = ResolutionTypes.FullHD,
                CaptureInterval = null
            });
        
        #endregion

        #region Act

        var response = await _getProcessingOrder.GetById(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        
        response!.Id.Should().Be(request.OrderId);
        
        response.Videos.Should().NotBeEmpty();        
        
        response.Videos.First().Id.Should().Be(videoMetadata.Id);
        response.Videos.First().Name.Should().Be(videoMetadata.Name);
        response.Videos.First().ContentType.Should().Be(videoMetadata.ContentType);
        response.Videos.First().Size.Should().Be(videoMetadata.Size);
        
        #endregion
    }
}