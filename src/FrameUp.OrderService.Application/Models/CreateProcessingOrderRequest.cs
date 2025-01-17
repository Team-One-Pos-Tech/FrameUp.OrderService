namespace FrameUp.OrderService.Application.Models;

public record CreateProcessingOrderRequest
{
    public Stream Video { get; set; }
    
    public VideoMetadataRequest VideoMetadata { get; set; }
    
    public IEnumerable<VideoRequest> Videos { get; init; } = new List<VideoRequest>();
}

public record VideoRequest
{
    public required Stream ContentStream { get; init; }
    
    public required VideoMetadataRequest Metadata { get; init; }
}

public record VideoMetadataRequest
{   
    public string Name { get; set; }
    
    public long Size { get; set; }
    
    public string ContentType { get; set; }
}