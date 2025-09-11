using Inventory.Application.DTOs.Messages;
using Inventory.Application.Services.MessageBus;
using Inventory.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Inventory.Application.Services.Consumers;

public class StockValidationConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockValidationConsumerService> _logger;
    private readonly IMessageBus _messageBus;

    public StockValidationConsumerService(
        IServiceProvider serviceProvider,
        ILogger<StockValidationConsumerService> logger,
        IMessageBus messageBus)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Validation Consumer Service started.");

        await _messageBus.ConsumeAsync<StockValidationMessage>(
            "stock-validation-queue",
            ProcessStockValidationMessage,
            stoppingToken);
    }

    private async Task ProcessStockValidationMessage(StockValidationMessage message)
    {
        try
        {
            _logger.LogInformation($"Processing stock validation for Order ID: {message.OrderId}");

            using var scope = _serviceProvider.CreateScope();
            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            var validatedItems = new List<OrderValidatedItem>();
            bool allItemsValid = true;

            foreach (var item in message.Items)
            {
                var product = await productRepository.GetAsync(item.ProductId);
                
                if (product == null)
                {
                    _logger.LogWarning($"Product not found: {item.ProductId}");
                    validatedItems.Add(new OrderValidatedItem(
                        item.ProductId,
                        item.Quantity,
                        0,
                        false
                    ));
                    allItemsValid = false;
                    continue;
                }

                bool hasStock = product.IsStockAvailable(item.Quantity);
                
                validatedItems.Add(new OrderValidatedItem(
                    item.ProductId,
                    item.Quantity,
                    product.Price,
                    hasStock
                ));

                if (!hasStock)
                {
                    allItemsValid = false;
                    _logger.LogWarning($"Insufficient stock for Product ID: {item.ProductId}. Required: {item.Quantity}, Available: {product.Stock}");
                }
            }

            var orderValidatedMessage = new OrderValidatedMessage(
                message.OrderId,
                validatedItems,
                allItemsValid
            );

            await messageBus.PublishAsync("order-validated", orderValidatedMessage);
            
            _logger.LogInformation("Stock validation completed for Order ID: {OrderId}. Status: {Status}", 
                message.OrderId, allItemsValid ? "VALID" : "INVALID");
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error processing stock validation message for Order ID: {OrderId}", message.OrderId);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Validation Consumer Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
