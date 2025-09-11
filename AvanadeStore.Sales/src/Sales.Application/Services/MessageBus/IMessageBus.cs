namespace Sales.Application.Services.MessageBus;
public interface IMessageBus
{
    Task PublishAsync<T>(string queue, T message) where T : class;
    Task ConsumeAsync<T>(string queue, Func<T, Task> onMessage, CancellationToken cancellationToken = default) where T : class;
}
