namespace FrameUp.OrderService.Api.Configuration;

public record MinIOSettings
{
    public string? Endpoint { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? BucketName { get; set; }

}