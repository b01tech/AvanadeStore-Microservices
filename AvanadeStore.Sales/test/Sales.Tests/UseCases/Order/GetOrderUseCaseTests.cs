using Moq;
using Sales.Application.DTOs.Responses;
using Sales.Application.UseCases.Order;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;

namespace Sales.Tests.UseCases.Order;

public class GetOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly GetOrderUseCase _getOrderUseCase;

    public GetOrderUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _getOrderUseCase = new GetOrderUseCase(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingOrder_ShouldReturnOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(userId);
        order.AddOrderItem(1L, 2, 10.0m);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _getOrderUseCase.ExecuteAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(order.Total, result.Total);
        Assert.Equal(order.Status, result.Status);
        Assert.Single(result.OrderItems);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingOrder_ShouldThrowOrderNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Sales.Domain.Entities.Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(() => _getOrderUseCase.ExecuteAsync(orderId));

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetAllAsync_WithOrders_ShouldReturnPaginatedList()
    {
        // Arrange
        var orders = new List<Sales.Domain.Entities.Order>();
        for (int i = 0; i < 15; i++)
        {
            var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
            order.AddOrderItem(1L, 1, 10.0m);
            orders.Add(order);
        }

        _orderRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _getOrderUseCase.ExecuteGetAllAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Orders.Count); // First page should have 10 items
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(2, result.TotalPages); // 15 items / 10 per page = 2 pages

        _orderRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetAllAsync_SecondPage_ShouldReturnRemainingItems()
    {
        // Arrange
        var orders = new List<Sales.Domain.Entities.Order>();
        for (int i = 0; i < 15; i++)
        {
            var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
            order.AddOrderItem(1L, 1, 10.0m);
            orders.Add(order);
        }

        _orderRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _getOrderUseCase.ExecuteGetAllAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Orders.Count); // Second page should have 5 remaining items
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(2, result.TotalPages);

        _orderRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetAllAsync_EmptyList_ShouldReturnEmptyPaginatedList()
    {
        // Arrange
        var orders = new List<Sales.Domain.Entities.Order>();

        _orderRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _getOrderUseCase.ExecuteGetAllAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Orders);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);

        _orderRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetByUserIdAsync_WithUserOrders_ShouldReturnUserOrders()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orders = new List<Sales.Domain.Entities.Order>();
        for (int i = 0; i < 5; i++)
        {
            var order = new Sales.Domain.Entities.Order(userId);
            order.AddOrderItem(1L, 1, 10.0m);
            orders.Add(order);
        }

        _orderRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(orders);

        // Act
        var result = await _getOrderUseCase.ExecuteGetByUserIdAsync(userId, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Orders.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(5, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        Assert.All(result.Orders, order => Assert.Equal(userId, order.UserId));

        _orderRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetByStatusAsync_WithStatusOrders_ShouldReturnOrdersWithStatus()
    {
        // Arrange
        var status = OrderStatus.Confirmed;
        var orders = new List<Sales.Domain.Entities.Order>();
        for (int i = 0; i < 3; i++)
        {
            var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
            order.AddOrderItem(1L, 1, 10.0m);
            
            // Use reflection to set the status since it's private set
            var statusProperty = typeof(Sales.Domain.Entities.Order).GetProperty("Status");
            statusProperty?.SetValue(order, status);
            
            orders.Add(order);
        }

        _orderRepositoryMock.Setup(x => x.GetByStatusAsync(status))
            .ReturnsAsync(orders);

        // Act
        var result = await _getOrderUseCase.ExecuteGetByStatusAsync(status, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Orders.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        Assert.All(result.Orders, order => Assert.Equal(status, order.Status));

        _orderRepositoryMock.Verify(x => x.GetByStatusAsync(status), Times.Once);
    }
}