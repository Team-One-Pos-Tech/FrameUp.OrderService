namespace FrameUp.OrderService.Application.Models;

public record FileBucketRequest
{
    public Guid OrderId { get; set; }

    public IEnumerable<FileRequest> Files { get; set; } = new List<FileRequest>();
}
