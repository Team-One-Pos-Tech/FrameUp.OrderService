using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models.Requests;

public record CreateProcessingOrderRequest
{
    public int? CaptureInterval { get; set; }

    public Guid OwnerId { get; set; }

    public ResolutionTypes? ExportResolution { get; set; }

    public IEnumerable<VideoRequest> Videos { get; init; } = [];
}

public record VideoRequest
{
    public required Stream ContentStream { get; init; }

    public required VideoMetadataRequest Metadata { get; init; }
}

public record VideoMetadataRequest
{
    public required string Name { get; init; }

    public long Size { get; init; }

    public required string ContentType { get; init; }
}