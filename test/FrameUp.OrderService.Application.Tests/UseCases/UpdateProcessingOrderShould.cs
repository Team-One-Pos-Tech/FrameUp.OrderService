using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.UseCases;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Infra.Repositories;
using MassTransit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Status = ProcessingStatus.Concluded,
        };

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
}
