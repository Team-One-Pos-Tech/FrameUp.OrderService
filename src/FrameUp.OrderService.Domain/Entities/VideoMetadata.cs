namespace FrameUp.OrderService.Domain.Entities;

public class VideoMetadata
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string ContentType { get; set; }
    
    public long Size { get; set; }

    public Guid OrderId { get; set; }

}