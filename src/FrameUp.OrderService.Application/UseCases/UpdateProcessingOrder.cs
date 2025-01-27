using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;

namespace FrameUp.OrderService.Application.UseCases;

public class UpdateProcessingOrder(IOrderRepository orderRepository, IPublishEndpoint publishEndpoint) : IUpdateProcessingOrder
{

    public async Task<UpdateProcessingOrderResponse> Execute(UpdateProcessingOrderRequest request)
    {
        var response = new UpdateProcessingOrderResponse();

        var order = await orderRepository.Get(request.OrderId);

        if (order is null)
        {
            response.AddNotification("Order", "Order not found");
            return response;
        }

        UpdateOrder(request, order);

        await PublishOrderStatusChangedEvent(order);

        await orderRepository.Update(order);

        return response;
    }

    private static void UpdateOrder(UpdateProcessingOrderRequest request, Order order)
    {
        order.Status = request.Status;
        order.PackageUri = request.PackageUri;
    }

    private async Task PublishOrderStatusChangedEvent(Order order)
    {
        await publishEndpoint.Publish(
            new OrderStatusChangedEvent(order.OwnerId, new OrderStatusChangedEventParameters
            {
                OrderId = order.Id,
                Status = order.Status
            })
        );
    }
}
