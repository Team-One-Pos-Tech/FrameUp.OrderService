namespace FrameUp.OrderService.Api.Configuration;

public record PostgreSQLSettings
{
    public string Host { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
}