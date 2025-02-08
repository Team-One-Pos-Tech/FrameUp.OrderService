using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Application.Validators;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Application.UseCases;

public class UpdateProcessingOrder(
    ILogger<UpdateProcessingOrder> logger,
    IOrderRepository orderRepository, 
    IPublishEndpoint publishEndpoint) : IUpdateProcessingOrder
{

    public async Task<UpdateProcessingOrderResponse> Execute(UpdateProcessingOrderRequest request)
    {
        logger.LogInformation("Starting update of Processing Order [{orderId}]", request.OrderId);
        
        var response = new UpdateProcessingOrderResponse();

        var order = await orderRepository.Get(request.OrderId);

        if (!UpdateProcessingOrderValidator.IsValid(order, request, out UpdateProcessingOrderResponse responseOut))
            return responseOut;

        UpdateOrder(request, order!);

        await PublishOrderStatusChangedEvent(order!);

        await orderRepository.Update(order!);
        
        logger.LogInformation("Processing Order [{orderId}] updated successfully!", request.OrderId);

        return response;
    }

    private static void UpdateOrder(UpdateProcessingOrderRequest request, Order order)
    {
        order.Status = request.Status;
        order.Packages = request.Packages.Select(package => new PackageItem(package.FileName, package.Uri)).ToList();
    }

    private async Task PublishOrderStatusChangedEvent(Order order)
    {
        await publishEndpoint.Publish(
            new OrderStatusChangedEvent(order.OwnerId, new OrderStatusChangedEventParameters
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                Packages = order.Packages.Select(package => new PackageItemResponse(package.FileName, package.Uri)).ToArray()
            })
        );
    }
}
