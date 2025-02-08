using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models.Requests;

public class UpdateProcessingOrderRequest
{
    public required Guid OrderId { get; init; }
    public required ProcessingStatus Status { get; init; }
    public UpdatePackageItemRequest[] Packages { get; set; } = [];
}

public record UpdatePackageItemRequest(string FileName, string Uri);