namespace FrameUp.OrderService.Api.Configuration;
public record Settings
{
    public required RabbitMQSettings RabbitMQ { get; set; }
    public required PostgreSQLSettings PostgreSQL { get; set; }
}