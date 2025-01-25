namespace FrameUp.OrderService.Application.Models;

public class VideoMetadataResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ContentType { get; set; }

    public long Size { get; set; }
}
