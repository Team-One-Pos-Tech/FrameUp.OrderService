namespace FrameUp.OrderService.Domain.Enums;

public enum ProcessingStatus
{
    Refused,
    Received,
    Uploading,
    Processing,
    Concluded,
    Canceled,
    Failed
}