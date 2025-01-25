using FluentAssertions;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.UseCases;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using Moq;

namespace FrameUp.OrderService.Application.Tests.UseCases;

public class UpdateProcessingOrderShould
{
    private Mock<IOrderRepository> orderRepository;
    private UpdateProcessingOrder updateProcessingOrder;

    [SetUp]
    public void Setup()
    {
        orderRepository = new Mock<IOrderRepository>();

        updateProcessingOrder = new UpdateProcessingOrder(orderRepository.Object);
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

        orderRepository.Setup(x => x.Get(request.OrderId))
            .ReturnsAsync(new Order
            {
                Id = request.OrderId,
                Status = ProcessingStatus.Processing,
            });

        #endregion

        #region Act

        await updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        orderRepository.Verify(x => x.Update(
            It.Is<Order>(order => order.Status == request.Status &&
                order.Id == request.OrderId)
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

        var response = await updateProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();

        response.Notifications.First().Message.Should().Be("Order not found");

        orderRepository.Verify(x => x.Update(
            It.Is<Order>(order => order.Status == request.Status &&
                order.Id == request.OrderId)
            ), Times.Never);

        #endregion
    }
}
