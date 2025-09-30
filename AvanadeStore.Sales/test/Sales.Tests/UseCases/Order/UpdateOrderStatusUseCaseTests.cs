using Moq;
using Sales.Application.DTOs.Messages;
using Sales.Application.DTOs.Responses;
using Sales.Application.Services.MessageBus;
using Sales.Application.UseCases.Order;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;

namespace Sales.Tests.UseCases.Order;

public class UpdateOrderStatusUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly UpdateOrderStatusUseCase _updateOrderStatusUseCase;

    public UpdateOrderStatusUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messageBusMock = new Mock<IMessageBus>();
        _updateOrderStatusUseCase = new UpdateOrderStatusUseCase(_orderRepositoryMock.Object, _unitOfWorkMock.Object, _messageBusMock.Object);
    }

    [Fact]
    public async Task ExecuteConfirmSeparationAsync_ValidConfirmedOrder_ShouldStartSeparation()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
        order.AddOrderItem(1L, 2, 10.0m);
        
        // Use reflection to set the status to Confirmed
        var statusProperty = typeof(Sales.Domain.Entities.Order).GetProperty("Status");
        statusProperty?.SetValue(order, OrderStatus.Confirmed);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _updateOrderStatusUseCase.ExecuteConfirmSeparationAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.InSeparation, result.Status);
        Assert.Equal(order.Id, result.Id);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteConfirmSeparationAsync_OrderNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Sales.Domain.Entities.Order?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _updateOrderStatusUseCase.ExecuteConfirmSeparationAsync(orderId));
        Assert.Equal("Order not found", exception.Message);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData(OrderStatus.Created)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.InSeparation)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Finished)]
    public async Task ExecuteConfirmSeparationAsync_InvalidStatus_ShouldThrowInvalidOrderStatusException(OrderStatus status)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
        order.AddOrderItem(1L, 2, 10.0m);
        
        // Use reflection to set the status
        var statusProperty = typeof(Sales.Domain.Entities.Order).GetProperty("Status");
        statusProperty?.SetValue(order, status);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOrderStatusException>(() => _updateOrderStatusUseCase.ExecuteConfirmSeparationAsync(orderId));

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.InSeparation)]
    public async Task ExecuteCancelOrderAsync_ValidStatusAndUser_ShouldCancelOrder(OrderStatus status)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(userId);
        order.AddOrderItem(1L, 2, 10.0m);
        
        // Use reflection to set the status
        var statusProperty = typeof(Sales.Domain.Entities.Order).GetProperty("Status");
        statusProperty?.SetValue(order, status);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _updateOrderStatusUseCase.ExecuteCancelOrderAsync(orderId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Cancelled, result.Status);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(userId, result.UserId);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteCancelOrderAsync_OrderNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Sales.Domain.Entities.Order?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _updateOrderStatusUseCase.ExecuteCancelOrderAsync(orderId, userId));
        Assert.Equal("Order not found", exception.Message);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteCancelOrderAsync_DifferentUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(differentUserId);
        order.AddOrderItem(1L, 2, 10.0m);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _updateOrderStatusUseCase.ExecuteCancelOrderAsync(orderId, userId));

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteFinishOrderAsync_ValidInSeparationOrder_ShouldFinishOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
        order.AddOrderItem(1L, 2, 10.0m);
        
        // Use reflection to set the status to InSeparation
        var statusProperty = typeof(Sales.Domain.Entities.Order).GetProperty("Status");
        statusProperty?.SetValue(order, OrderStatus.InSeparation);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _updateOrderStatusUseCase.ExecuteFinishOrderAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Finished, result.Status);
        Assert.Equal(order.Id, result.Id);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<OrderFinishedMessage>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteFinishOrderAsync_OrderNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Sales.Domain.Entities.Order?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _updateOrderStatusUseCase.ExecuteFinishOrderAsync(orderId));
        Assert.Equal("Order not found", exception.Message);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<OrderFinishedMessage>()), Times.Never);
    }

    [Theory]
    [InlineData(OrderStatus.Created)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Finished)]
    public async Task ExecuteFinishOrderAsync_InvalidStatus_ShouldThrowInvalidOrderStatusException(OrderStatus status)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Sales.Domain.Entities.Order(Guid.NewGuid());
        order.AddOrderItem(1L, 2, 10.0m);

        var statusProperty = typeof(Sales.Domain.Entities.Order).GetProperty("Status");
        statusProperty?.SetValue(order, status);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOrderStatusException>(() => _updateOrderStatusUseCase.ExecuteFinishOrderAsync(orderId));

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<OrderFinishedMessage>()), Times.Never);
    }
}