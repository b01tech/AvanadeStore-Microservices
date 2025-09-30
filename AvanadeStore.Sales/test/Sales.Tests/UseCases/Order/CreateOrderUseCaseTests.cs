using Moq;
using Sales.Application.DTOs.Requests;
using Sales.Application.DTOs.Responses;
using Sales.Application.DTOs.Messages;
using Sales.Application.Services.MessageBus;
using Sales.Application.UseCases.Order;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;

namespace Sales.Tests.UseCases.Order;

public class CreateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly CreateOrderUseCase _createOrderUseCase;

    public CreateOrderUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messageBusMock = new Mock<IMessageBus>();
        _createOrderUseCase = new CreateOrderUseCase(_orderRepositoryMock.Object, _unitOfWorkMock.Object, _messageBusMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var orderItems = new List<RequestOrderItemDTO>
        {
            new RequestOrderItemDTO(1L, 2),
            new RequestOrderItemDTO(2L, 1)
        };
        var request = new RequestCreateOrderDTO(orderItems);

        var createdOrder = new Sales.Domain.Entities.Order();
        createdOrder.AddOrderItem(1L, 2);
        createdOrder.AddOrderItem(2L, 1);

        _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()))
            .ReturnsAsync(createdOrder);

        // Act
        var result = await _createOrderUseCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdOrder.Id, result.Id);
        Assert.Equal(createdOrder.UserId, result.UserId);
        Assert.Equal(createdOrder.Total, result.Total);
        Assert.Equal(createdOrder.Status, result.Status);
        Assert.Equal(2, result.OrderItems.Count);

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<StockValidationMessage>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithUserId_ValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orderItems = new List<RequestOrderItemDTO>
        {
            new RequestOrderItemDTO(1L, 2)
        };
        var request = new RequestCreateOrderDTO(orderItems);

        var createdOrder = new Sales.Domain.Entities.Order(userId);
        createdOrder.AddOrderItem(1L, 2);

        _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()))
            .ReturnsAsync(createdOrder);

        // Act
        var result = await _createOrderUseCase.ExecuteAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdOrder.Id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(createdOrder.Total, result.Total);
        Assert.Equal(createdOrder.Status, result.Status);
        Assert.Single(result.OrderItems);

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<StockValidationMessage>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyOrderItems_ShouldThrowInvalidArgumentsException()
    {
        // Arrange
        var request = new RequestCreateOrderDTO(new List<RequestOrderItemDTO>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidArgumentsException>(() => _createOrderUseCase.ExecuteAsync(request));

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<StockValidationMessage>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NullOrderItems_ShouldThrowInvalidArgumentsException()
    {
        // Arrange
        var request = new RequestCreateOrderDTO(null!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidArgumentsException>(() => _createOrderUseCase.ExecuteAsync(request));

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<StockValidationMessage>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecuteAsync_InvalidQuantity_ShouldThrowInvalidArgumentsException(int quantity)
    {
        // Arrange
        var orderItems = new List<RequestOrderItemDTO>
        {
            new RequestOrderItemDTO(1L, quantity)
        };
        var request = new RequestCreateOrderDTO(orderItems);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidArgumentsException>(() => _createOrderUseCase.ExecuteAsync(request));

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<StockValidationMessage>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var orderItems = new List<RequestOrderItemDTO>
        {
            new RequestOrderItemDTO(1L, 2)
        };
        var request = new RequestCreateOrderDTO(orderItems);

        _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()))
            .ThrowsAsync(new System.Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<System.Exception>(() => _createOrderUseCase.ExecuteAsync(request));
        Assert.Equal("Database error", exception.Message);

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sales.Domain.Entities.Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<StockValidationMessage>()), Times.Never);
    }
}