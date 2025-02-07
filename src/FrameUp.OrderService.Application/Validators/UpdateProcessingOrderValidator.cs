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

        if (request.Status == ProcessingStatus.Canceled && order.Status != ProcessingStatus.Processing)
        {
            responseOut.AddNotification("Status", "Just orders in processing status can be canceled.");
            return false;
        }

        return true;
    }
}