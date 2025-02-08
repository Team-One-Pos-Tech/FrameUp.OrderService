using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Entities;
using MassTransit.Transports;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Application.UseCases;

public class GetProcessingOrder(ILogger<GetProcessingOrder> logger, IOrderRepository orderRepository) : IGetProcessingOrder
{
    public async Task<IEnumerable<GetProcessingOrderResponse>> GetAll(GetProcessingOrderRequest request)
    {
        logger.LogInformation("Retrieving all processing orders for owner [{ownerId}]", request.RequesterId);
        
        var orders = await orderRepository.GetAll(request.RequesterId);

        return orders.Select(CreateOrderResponse);
    }

    public async Task<GetProcessingOrderResponse> GetById(GetProcessingOrderRequest request)
    {
        logger.LogInformation("Retrieving a processing order for owner [{ownerId}]", request.RequesterId);
        
        var order = await orderRepository.Get(request.OrderId);

        if(!IsValid(order, request, out var response))
        {
            return response;
        }

        return CreateOrderResponse(order!);
    }

    public static bool IsValid(Order? order, GetProcessingOrderRequest request, out GetProcessingOrderResponse response)
    {
        response = new GetProcessingOrderResponse();

        if (order is null)
        {
            response.AddNotification("Order", "Order not found");
            return false;
        }

        if (order.OwnerId != request.RequesterId)
        {
            response.AddNotification("Order", "Requester has no permission to the order");
            return false;
        }

        return true;
    }

    private static GetProcessingOrderResponse CreateOrderResponse(Order order)
    {
        return new GetProcessingOrderResponse
        {
            Id = order.Id,
            Status = order.Status,
            OwnerId = order.OwnerId,
            Videos = order.Videos.Select(video => new VideoMetadataResponse
            {
                Id = video.Id,
                Name = video.Name,
                ContentType = video.ContentType,
                Size = video.Size
            }),
            ExportResolution = order.ExportResolution,
            CaptureInterval = order.CaptureInterval,
            Packages = order.Packages.Select(package => new PackageItemResponse(package.FileName, package.Uri)).ToArray()
        };
    }
}
