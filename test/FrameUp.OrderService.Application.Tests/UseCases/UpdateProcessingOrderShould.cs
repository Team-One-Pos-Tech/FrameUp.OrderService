using FluentAssertions;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.UseCases;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;

namespace FrameUp.OrderService.Application.Tests.UseCases;

public class UpdateProcessingOrderShould
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private UpdateProcessingOrder _updateProcessingOrder;
    private Mock<IPublishEndpoint> _publishedEndpointMock;
    private Mock<ILogger<UpdateProcessingOrder>> _logger;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _logger = new Mock<ILogger<UpdateProcessingOrder>>();
        _publishedEndpointMock = new Mock<IPublishEndpoint>();

        _updateProcessingOrder = new UpdateProcessingOrder(
            _logger.Object,
            _orderRepositoryMock.Object,
            _publishedEndpointMock.Object
            );
    }

    [Test]
    public async Task Update_Status_Processing()
    {
        #region Arrange

        var request = new UpdateProcessingOrderRequest
        {
            OrderId = Guid.NewGuid(),
            Status = ProcessingStatus.Concluded
        };

        _orderRepositoryMock.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing,
            });

        #endregion

        #region Act

        await _updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        _orderRepositoryMock.Verify(x => x.Update(
            It.Is<Order>(order => order.Status == request.Status &&
                order.Id == request.OrderId)
            ), Times.Once);

        #endregion
    }
    
    [Test]
    public async Task Publish_Event_When_Order_Status_Is_Changed()
    {
        #region Arrange

        var ownerId = Guid.NewGuid();

        var request = new UpdateProcessingOrderRequest
        {
            OrderId = Guid.NewGuid(),
            Status = ProcessingStatus.Concluded
        };

        _orderRepositoryMock.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing,
                OwnerId = ownerId
            });

        #endregion

        #region Act

        await _updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        _publishedEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(
            It.Is<OrderStatusChangedEvent>(message => message.OwnerId == ownerId
                && message.Parameters.OrderId == request.OrderId
                && message.Parameters.Status == request.Status.ToString()
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        #endregion
    }
    
    [Test]
    public async Task Publish_Event_With_Packages_When_Order_Status_Is_Changed()
    {
        #region Arrange

        var ownerId = Guid.NewGuid();

        var request = new UpdateProcessingOrderRequest
        {
            OrderId = Guid.NewGuid(),
            Status = ProcessingStatus.Concluded,
            Packages = [
                new UpdatePackageItemRequest("file1.mp4", "https://s3.com/file1")
            ]
        };

        _orderRepositoryMock.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing,
                OwnerId = ownerId
            });

        #endregion

        #region Act

        await _updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        _publishedEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(
            It.Is<OrderStatusChangedEvent>(
                message => message.Parameters.Packages.Length == request.Packages.Length &&
                message.Parameters.Packages.First().FileName == request.Packages.First().FileName &&
                message.Parameters.Packages.First().Uri == request.Packages.First().Uri
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        #endregion
    }
    
    [Test]
    public async Task Update_Order_With_Package()
    {
        #region Arrange

        var ownerId = Guid.NewGuid();

        var request = new UpdateProcessingOrderRequest
        {
            OrderId = Guid.NewGuid(),
            Status = ProcessingStatus.Concluded,
            Packages = [
                new UpdatePackageItemRequest("file1.mp4", "https://s3.com/file1")
            ]
        };

        _orderRepositoryMock.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing,
                OwnerId = ownerId
            });

        #endregion

        #region Act

        await _updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        _orderRepositoryMock.Verify(x => x.Update(
            It.Is<Order>(order => 
                order.Packages.Count == request.Packages.Length &&
                order.Packages.First().FileName == request.Packages.First().FileName &&
                order.Packages.First().Uri == request.Packages.First().Uri
                )
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Validate_When_Order_Does_Not_Exists()
    {
        #region Arrange

        var request = new UpdateProcessingOrderRequest
        {
            OrderId = Guid.NewGuid(),
            Status = ProcessingStatus.Concluded
        };

        #endregion

        #region Act

        var response = await _updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();

        response.Notifications.First().Message.Should().Be("Order not found");

        _orderRepositoryMock.Verify(x => x.Update(
            It.Is<Order>(order => order.Status == request.Status &&
                order.Id == request.OrderId)
            ), Times.Never);

        #endregion
    }

    [Test]
    public async Task Validate_When_Try_Cancel_Order_With_Status_Is_Not_Processing()
    {
        #region Arrange

        var request = new UpdateProcessingOrderRequest
        {
            OrderId = Guid.NewGuid(),
            Status = ProcessingStatus.Canceled
        };

        _orderRepositoryMock.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Concluded
            });

        #endregion

        #region Act

        var response = await _updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();

        response.Notifications.First().Message.Should().Be("Just orders in processing status can be canceled.");

        _orderRepositoryMock.Verify(x => x.Update(
            It.Is<Order>(order => order.Status == request.Status &&
                order.Id == request.OrderId)
            ), Times.Never);

        #endregion
    }
}
