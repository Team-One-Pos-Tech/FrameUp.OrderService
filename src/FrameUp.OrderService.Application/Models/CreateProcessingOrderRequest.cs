namespace FrameUp.OrderService.Application.Models;

public record CreateProcessingOrderRequest
{
    public required Stream Video { get; init; }
    public required VideoMetadataRequest VideoMetadata { get; init; }
}

public record VideoMetadataRequest
{
    public string Name { get; set; }
    
    public long Size { get; set; }
    
    public string ContentType { get; set; }
}