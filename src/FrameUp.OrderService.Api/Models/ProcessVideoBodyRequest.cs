using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Api.Models;

public class ProcessVideoBodyRequest
{
    public int? CaptureInterval { get; set; }

    public ResolutionTypes? ExportResolution { get; set; }

    public required IEnumerable<IFormFile> Videos { get; set; } = [];
}