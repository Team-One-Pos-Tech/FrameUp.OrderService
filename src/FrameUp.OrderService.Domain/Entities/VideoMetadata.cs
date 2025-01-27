namespace FrameUp.OrderService.Domain.Entities;

public class VideoMetadata
{
    public Guid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required string ContentType { get; init; }
    
    public long Size { get; init; }

    public Guid OrderId { get; set; }

}