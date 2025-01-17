using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }

    public VideoMetadata VideoMetadata { get; set; }
    
    public ProcessingStatus Status { get; set; }
    

    public Guid OwnerId { get; set; }

    public Order()
    {
    }
}