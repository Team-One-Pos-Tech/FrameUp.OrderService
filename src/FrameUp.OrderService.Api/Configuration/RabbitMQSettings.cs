namespace FrameUp.OrderService.Api.Configuration;

public record RabbitMQSettings
{
    public required string ConnectionString { get; set; }
}