using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Inventory.Exception.CustomExceptions;
using Inventory.Exception.ErrorMessages;
using System.Text;
using System.Text.Json;
using Inventory.Application.Services.MessageBus;

namespace Sales.Application.Services.MessageBus;
internal class RabbitMqMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private  IConnectionFactory _factory;
    private  IConnection _connection;
    private  IChannel _publishChannel;
    private  IChannel _consumeChannel;

    public RabbitMqMessageBus(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public async Task InitializeAsync()
    {
        _factory = CreateConnectionFactory(_settings);
        _connection = await _factory.CreateConnectionAsync();
        _publishChannel = await _connection.CreateChannelAsync();
        _consumeChannel = await _connection.CreateChannelAsync();
        await _publishChannel.BasicQosAsync(0, _settings.PrefetchCount, false);
        await _consumeChannel.BasicQosAsync(0, _settings.PrefetchCount, false);
        await Task.CompletedTask;
    }

    public async Task PublishAsync<T>(string queue, T message) where T : class
    {
        if (_publishChannel == null)
            throw new MessageFailException(ResourceErrorMessages.MESSAGEBUS_INITIALIZATION);

        await _publishChannel.QueueDeclareAsync(queue, true, false, false, null);

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var props = new BasicProperties { Persistent = true };

        await _publishChannel.BasicPublishAsync(
            exchange: "",
            routingKey: queue,
            mandatory: false,
            basicProperties: props,
            body: body);
    }
    public async Task ConsumeAsync<T>(string queue, Func<T, Task> onMessage, CancellationToken cancellationToken = default) where T : class
    {
        if (_consumeChannel == null)
            throw new MessageFailException(ResourceErrorMessages.MESSAGEBUS_INITIALIZATION);

        await _consumeChannel.QueueDeclareAsync(queue, true, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(_consumeChannel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var msgJson = Encoding.UTF8.GetString(body);
                var msg = JsonSerializer.Deserialize<T>(msgJson);

                if (msg != null)
                    await onMessage(msg);

                await _consumeChannel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (System.Exception ex)
            {
                await _consumeChannel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                Console.Error.WriteLine($"{ResourceErrorMessages.CONSUME_MESSAGE_FAIL}|{queue}: {ex}");
            }
        };

        await _consumeChannel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer);

        cancellationToken.Register(async () =>
        {
            if (_consumeChannel != null) await _consumeChannel.CloseAsync();
        });
    }
    private ConnectionFactory CreateConnectionFactory(RabbitMqSettings settings)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.User,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };
        return factory;
    }

    public async ValueTask DisposeAsync()
    {
        if (_publishChannel != null) await _publishChannel.DisposeAsync();
        if (_consumeChannel != null) await _consumeChannel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }
}
