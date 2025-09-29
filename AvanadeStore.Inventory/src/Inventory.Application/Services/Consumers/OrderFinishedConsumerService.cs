using Inventory.Application.DTOs.Messages;
using Inventory.Application.Services.MessageBus;
using Inventory.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Services.Consumers;

public class OrderFinishedConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderFinishedConsumerService> _logger;
    private readonly IMessageBus _messageBus;

    public OrderFinishedConsumerService(
        IServiceProvider serviceProvider,
        ILogger<OrderFinishedConsumerService> logger,
        IMessageBus messageBus)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Finished Consumer Service started.");

        await _messageBus.ConsumeAsync<OrderFinishedMessage>(
            QueueNames.ORDER_FINISHED_QUEUE,
            ProcessOrderFinishedMessage,
            stoppingToken);
    }

    private async Task ProcessOrderFinishedMessage(OrderFinishedMessage message)
    {
        try
        {
            _logger.LogInformation($"Processing order finished for Order ID: {message.OrderId}");

            using var scope = _serviceProvider.CreateScope();
            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            foreach (var item in message.Items)
            {
                var product = await productRepository.GetAsync(item.ProductId);

                if (product == null)
                {
                    _logger.LogWarning($"Product not found: {item.ProductId}");
                    continue;
                }

                product.DecreaseStock(item.Quantity);
                await productRepository.DecreaseStockAsync(product.Id, item.Quantity);

                _logger.LogInformation($"Stock decreased for Product ID: {item.ProductId}. Quantity: {item.Quantity}. New Stock: {product.Stock}");
            }

            await unitOfWork.CommitAsync();

            _logger.LogInformation("Order finished processing completed for Order ID: {OrderId}", message.OrderId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error processing order finished message for Order ID: {OrderId}", message.OrderId);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Finished Consumer Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}