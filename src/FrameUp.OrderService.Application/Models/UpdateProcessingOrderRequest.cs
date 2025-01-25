using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models;

public class UpdateProcessingOrderRequest
{
    public Guid OrderId { get; set; }
    public ProcessingStatus Status { get; set; }
    public Guid OwnerId { get; set; }
}