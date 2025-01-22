using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    
    public ProcessingStatus Status { get; set; }
    
    public Guid OwnerId { get; set; }

    public ICollection<VideoMetadata> Videos { get; set; } = new List<VideoMetadata>();

    public ResolutionTypes ExportResolution { get; set; }

    public int? CaptureInterval { get; set; }

    public Order()
    {
    }
}