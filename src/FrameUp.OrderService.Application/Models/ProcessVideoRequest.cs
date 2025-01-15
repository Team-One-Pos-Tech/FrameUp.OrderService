namespace FrameUp.OrderService.Application.Models;

public record ProcessVideoRequest
{
    public required Stream Video { get; init; }
}