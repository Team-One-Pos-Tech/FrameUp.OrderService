namespace FrameUp.OrderService.Api.Configuration;

public record PostgreSQLSettings
{
    public string ConnectionString { get; set; }
}