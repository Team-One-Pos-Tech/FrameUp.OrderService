using FrameUp.OrderService.Domain.Enums;
using MassTransit;

namespace FrameUp.OrderService.Application.Models.Events;

[MessageUrn("frameup-order-service")]
[EntityName("update-order-status")]
public class UpdateOrderStatusEvent
{
    public Guid OrderId { get; set; }
    public ProcessingStatus Status { get; set; }
    public List<UploadedPackageItem> PackageItems { get; set; } = new List<UploadedPackageItem>();
}

public record UploadedPackageItem(string FileName, string Uri);