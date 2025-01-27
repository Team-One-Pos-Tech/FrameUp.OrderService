using FrameUp.OrderService.Domain.Enums;
using MassTransit;

namespace FrameUp.OrderService.Application.Models.Events;

[MessageUrn("frameup-order-service")]
[EntityName("order-status-changed")]
public record OrderStatusChangedEvent(Guid OwnerId, OrderStatusChangedEventParameters Parameters);

public record OrderStatusChangedEventParameters
{
    public Guid OrderId { get; set; }
    public ProcessingStatus Status { get; set; }
    public string? PackageLink { get; set; }
}