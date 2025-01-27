namespace FrameUp.OrderService.Application.Models.Requests;

public record FileRequest
{
    public required Stream ContentStream { get; init; }

    public required string Name { get; set; }

    public long Size { get; set; }

    public required string ContentType { get; set; }
}
