namespace Inventory.Application.Services.MessageBus;
public class RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5672;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ushort PrefetchCount { get; set; } = 10;
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
}
