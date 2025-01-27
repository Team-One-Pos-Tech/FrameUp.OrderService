namespace FrameUp.OrderService.Api.Configuration;

public record PostgreSQLSettings
{
    public required string ConnectionString { get; set; }
}