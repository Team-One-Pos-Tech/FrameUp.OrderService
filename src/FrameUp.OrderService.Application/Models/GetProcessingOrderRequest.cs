namespace FrameUp.OrderService.Application.Models;

public record GetProcessingOrderRequest
{
    public Guid OrderId { get; set; }

    public Guid OwnerId { get; set; }
}