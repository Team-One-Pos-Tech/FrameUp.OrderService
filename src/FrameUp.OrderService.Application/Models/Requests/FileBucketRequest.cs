namespace FrameUp.OrderService.Application.Models.Requests;

public record FileBucketRequest
{
    public Guid OrderId { get; set; }

    public IEnumerable<FileRequest> Files { get; set; } = new List<FileRequest>();
}
