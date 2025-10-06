using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sales.Application.DTOs.Messages;
using Sales.Application.Services.MessageBus;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;

namespace Sales.Application.Services.Consumers;

public class OrderValidationConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderValidationConsumerService> _logger;
    private readonly IMessageBus _messageBus;

    public OrderValidationConsumerService(
        IServiceProvider serviceProvider,
        ILogger<OrderValidationConsumerService> logger,
        IMessageBus messageBus)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Validation Consumer Service started.");

        await _messageBus.ConsumeAsync<OrderValidatedMessage>(
            QueueNames.ORDER_VALIDATED_QUEUE,
            ProcessOrderValidatedMessage,
            stoppingToken);
    }

    private async Task ProcessOrderValidatedMessage(OrderValidatedMessage message)
    {
        try
        {
            _logger.LogInformation("Processing order validation result for Order ID: {OrderId}", message.OrderId);

            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var order = await orderRepository.GetByIdAsync(message.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", message.OrderId);
                return;
            }

            foreach (var validatedItem in message.Items)
            {
                UpdatePrice(order, validatedItem);
            }

            UpdateOrderStatus(order, message);
            await orderRepository.UpdateAsync(order);
            await unitOfWork.CommitAsync();


            _logger.LogInformation("Order validation processing completed for Order ID: {OrderId}. Final Status: {Status}",
                message.OrderId, order.Status);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error processing order validation message for Order ID: {OrderId}", message.OrderId);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Validation Consumer Service is stopping.");
        await base.StopAsync(stoppingToken);
    }

    private void UpdatePrice(Order order, OrderValidatedItem validatedItem)
    {
        var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == validatedItem.ProductId);
        if (orderItem != null)
        {
            order.UpdateOrderItemPrice(validatedItem.ProductId, validatedItem.Price);
            _logger.LogInformation("Updated price for Product ID: {ProductId} to {Price}",
                validatedItem.ProductId, validatedItem.Price);
        }
    }
    private void UpdateOrderStatus(Order order, OrderValidatedMessage message)
    {
        if (message.IsValid)
        {
            order.ConfirmOrder();
            _logger.LogInformation("Order {OrderId} confirmed successfully", message.OrderId);
        }
        else
        {
            order.RejectOrder();
            _logger.LogWarning("Order {OrderId} rejected due to stock validation failure", message.OrderId);
        }

    }
}
