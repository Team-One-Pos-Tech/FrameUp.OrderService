using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models.Consumers;

public record ReadyToProcessVideo(Guid OrderId, ProcessVideoParameters Parameters);

public class ProcessVideoParameters
{
    public ResolutionTypes? ExportResolution { get; set; }

    public int? CaptureInterval { get; internal set; }
}