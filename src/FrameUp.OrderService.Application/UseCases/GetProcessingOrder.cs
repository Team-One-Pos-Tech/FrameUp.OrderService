using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.UseCases;

public class GetProcessingOrder(IOrderRepository orderRepository) : IGetProcessingOrder
{
    public async Task<IEnumerable<GetProcessingOrderResponse>> GetAll(GetProcessingOrderRequest request)
    {
        IEnumerable<Order> orders = await orderRepository.GetAll(request.OwnerId);

        return orders.Select(CreateOrderResponse);
    }

    public async Task<GetProcessingOrderResponse?> GetById(GetProcessingOrderRequest request)
    {
        var order = await orderRepository.Get(request.OrderId);

        if (order == null)
            return null;

        return CreateOrderResponse(order);
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
            CaptureInterval = order.CaptureInterval
        };
    }
}
