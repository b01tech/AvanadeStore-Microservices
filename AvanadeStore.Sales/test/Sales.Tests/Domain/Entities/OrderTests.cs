using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Exception.CustomExceptions;

namespace Sales.Tests.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_DefaultConstructor_ShouldCreateOrderWithDefaultValues()
    {
        // Act
        var order = new Order();

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(Guid.Empty, order.UserId);
        Assert.Equal(OrderStatus.Created, order.Status);
        Assert.Equal(0, order.Total);
        Assert.Empty(order.OrderItems);
        Assert.True(DateTime.UtcNow.Subtract(order.CreatedAt).Duration() < TimeSpan.FromSeconds(1));
        Assert.True(DateTime.UtcNow.Subtract(order.UpdatedAt).Duration() < TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithUserId_ShouldCreateOrderWithUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var order = new Order(userId);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(OrderStatus.Created, order.Status);
        Assert.Equal(0, order.Total);
        Assert.Empty(order.OrderItems);
        Assert.True(DateTime.UtcNow.Subtract(order.CreatedAt).Duration() < TimeSpan.FromSeconds(1));
        Assert.True(DateTime.UtcNow.Subtract(order.UpdatedAt).Duration() < TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddOrderItem_NewProduct_ShouldAddOrderItem()
    {
        // Arrange
        var order = new Order();
        var productId = 1L;
        var quantity = 2;
        var price = 10.0m;

        // Act
        order.AddOrderItem(productId, quantity, price);

        // Assert
        Assert.Single(order.OrderItems);
        var orderItem = order.OrderItems.First();
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(price, orderItem.Price);
        Assert.Equal(20.0m, order.Total); // 2 * 10.0
    }

    [Fact]
    public void AddOrderItem_ExistingProduct_ShouldUpdateQuantity()
    {
        // Arrange
        var order = new Order();
        var productId = 1L;
        var initialQuantity = 2;
        var additionalQuantity = 3;
        var price = 10.0m;

        // Act
        order.AddOrderItem(productId, initialQuantity, price);
        order.AddOrderItem(productId, additionalQuantity, price);

        // Assert
        Assert.Single(order.OrderItems);
        var orderItem = order.OrderItems.First();
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(initialQuantity + additionalQuantity, orderItem.Quantity);
        Assert.Equal(price, orderItem.Price);
        Assert.Equal(50.0m, order.Total); // 5 * 10.0
    }

    [Fact]
    public void AddOrderItem_WithoutPrice_ShouldAddOrderItemWithDefaultPrice()
    {
        // Arrange
        var order = new Order();
        var productId = 1L;
        var quantity = 2;

        // Act
        order.AddOrderItem(productId, quantity);

        // Assert
        Assert.Single(order.OrderItems);
        var orderItem = order.OrderItems.First();
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(1m, orderItem.Price);
        Assert.Equal(2.0m, order.Total); // 2 * 1.0
    }

    [Fact]
    public void RemoveOrderItem_ExistingProduct_ShouldRemoveOrderItem()
    {
        // Arrange
        var order = new Order();
        var productId = 1L;
        var quantity = 2;
        var price = 10.0m;

        order.AddOrderItem(productId, quantity, price);

        // Act
        order.RemoveOrderItem(productId);

        // Assert
        Assert.Empty(order.OrderItems);
        Assert.Equal(0, order.Total);
    }

    [Fact]
    public void UpdateOrderItemQuantity_ExistingProduct_ShouldUpdateQuantity()
    {
        // Arrange
        var order = new Order();
        var productId = 1L;
        var initialQuantity = 2;
        var newQuantity = 5;
        var price = 10.0m;

        order.AddOrderItem(productId, initialQuantity, price);

        // Act
        order.UpdateOrderItemQuantity(productId, newQuantity);

        // Assert
        Assert.Single(order.OrderItems);
        var orderItem = order.OrderItems.First();
        Assert.Equal(newQuantity, orderItem.Quantity);
        Assert.Equal(50.0m, order.Total); // 5 * 10.0
    }

    [Fact]
    public void UpdateOrderItemPrice_ExistingProduct_ShouldUpdatePrice()
    {
        // Arrange
        var order = new Order();
        var productId = 1L;
        var quantity = 2;
        var initialPrice = 10.0m;
        var newPrice = 15.0m;

        order.AddOrderItem(productId, quantity, initialPrice);

        // Act
        order.UpdateOrderItemPrice(productId, newPrice);

        // Assert
        Assert.Single(order.OrderItems);
        var orderItem = order.OrderItems.First();
        Assert.Equal(newPrice, orderItem.Price);
        Assert.Equal(30.0m, order.Total); // 2 * 15.0
    }


    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.InSeparation)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Finished)]
    public void RemoveOrderItem_OrderNotInCreatedStatus_ShouldThrowInvalidOrderStatusException(OrderStatus status)
    {
        // Arrange
        var order = new Order();
        order.AddOrderItem(1L, 1, 10.0m); // Add item first

        // Use reflection to set the status since it's private set
        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty?.SetValue(order, status);

        // Act & Assert
        Assert.Throws<InvalidOrderStatusException>(() => order.RemoveOrderItem(1L));
    }
}
