namespace FrameUp.OrderService.Api.Configuration;

public record MinIOSettings
{
    public required string Endpoint { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }

}