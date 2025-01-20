namespace FrameUp.OrderService.Api.Models;

public class ProcessVideoBodyRequest
{
    public int Resolution { get; set; }

    public IEnumerable<IFormFile> Videos { get; set; } = [];
}