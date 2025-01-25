namespace FrameUp.OrderService.Application.Models.Requests;

public record FileRequest
{
    public Stream ContentStream { get; init; }

    public string Name { get; set; }

    public long Size { get; set; }

    public string ContentType { get; set; }
}
