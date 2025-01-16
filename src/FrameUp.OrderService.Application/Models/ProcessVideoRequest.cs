namespace FrameUp.OrderService.Application.Models;

public record ProcessVideoRequest
{
    public required Stream Video { get; init; }
    public string VideoName { get; set; }
    public long VideoSize { get; set; }
}