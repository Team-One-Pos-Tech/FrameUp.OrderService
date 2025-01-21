using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.UseCases;

public class GetProcessingOrder(IOrderRepository orderRepository) : IGetProcessingOrder
{
    public async Task<GetProcessingOrderResponse?> GetById(GetProcessingOrderRequest request)
    {
        var order = await orderRepository.Get(request.OrderId, Guid.Empty);

        if (order == null)
            return null;

        return new GetProcessingOrderResponse
        {
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
