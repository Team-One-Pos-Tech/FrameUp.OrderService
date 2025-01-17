namespace FrameUp.OrderService.Api.Models;

public class ProcessVideoBodyRequest
{
    public IEnumerable<IFormFile> Videos { get; set; } = [];
}