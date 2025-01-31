namespace FrameUp.OrderService.Application.Models.Requests;

public record GetProcessingOrderRequest
{
    public Guid OrderId { get; set; }

    public Guid OwnerId { get; set; }
    public Guid RequesterId { get; set; }
}