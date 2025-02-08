using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;

namespace FrameUp.OrderService.Application.Models.Events;

[MessageUrn("frameup-order-service")]
[EntityName("order-status-changed")]
public record OrderStatusChangedEvent(Guid OwnerId, OrderStatusChangedEventParameters Parameters);

public record OrderStatusChangedEventParameters
{
    public Guid OrderId { get; set; }
    public required string Status { get; set; }
    public PackageItemResponse[] Packages { get; set; } = [];
}