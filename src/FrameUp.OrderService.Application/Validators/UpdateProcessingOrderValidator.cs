using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;

namespace FrameUp.OrderService.Application.Validators;

public static class UpdateProcessingOrderValidator
{

    public static bool IsValid(Order? order, UpdateProcessingOrderRequest request, out UpdateProcessingOrderResponse responseOut)
    {
        responseOut = new UpdateProcessingOrderResponse();

        if (order is null)
        {
            responseOut.AddNotification("Order", "Order not found");
            return false;
        }

        if (CanCancelOrder(order, request))
        {
            responseOut.AddNotification("Status", "Order already processed cannot be cancelled.");
            return false;
        }

        return true;
    }

    private static bool CanCancelOrder(Order order, UpdateProcessingOrderRequest request)
    {
        return request.Status == ProcessingStatus.Canceled && (
            order.Status == ProcessingStatus.Canceled || 
            order.Status == ProcessingStatus.Failed ||
            order.Status == ProcessingStatus.Concluded ||
            order.Status == ProcessingStatus.Refused);
    }
}