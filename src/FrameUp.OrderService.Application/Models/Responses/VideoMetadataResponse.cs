namespace FrameUp.OrderService.Application.Models.Responses;

public class VideoMetadataResponse
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string ContentType { get; init; }

    public long Size { get; init; }
}
