using Sales.Domain.Entities;
using Sales.Exception.CustomExceptions;

namespace Sales.Tests.Domain.Entities;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateOrderItemSuccessfully()
    {
        // Arrange
        var productId = 1L;
        var quantity = 2;
        var price = 10.0m;

        // Act
        var orderItem = new OrderItem(productId, quantity, price);

        // Assert
        Assert.NotEqual(Guid.Empty, orderItem.Id);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(price, orderItem.Price);
    }

    [Theory]
    [InlineData(0, 1, 10.0)]
    [InlineData(-1, 1, 10.0)]
    [InlineData(1, 0, 10.0)]
    [InlineData(1, -1, 10.0)]
    [InlineData(1, 1, 0.0)]
    [InlineData(1, 1, -1.0)]
    public void Constructor_InvalidParameters_ShouldThrowOnValidationException(long productId, int quantity, decimal price)
    {
        // Act & Assert
        Assert.Throws<OnValidationException>(() => new OrderItem(productId, quantity, price));
    }

    [Fact]
    public void UpdateQuantity_ValidQuantity_ShouldUpdateQuantity()
    {
        // Arrange
        var orderItem = new OrderItem(1L, 2, 10.0m);
        var newQuantity = 5;

        // Act
        orderItem.UpdateQuantity(newQuantity);

        // Assert
        Assert.Equal(newQuantity, orderItem.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateQuantity_InvalidQuantity_ShouldThrowOnValidationException(int quantity)
    {
        // Arrange
        var orderItem = new OrderItem(1L, 2, 10.0m);

        // Act & Assert
        Assert.Throws<OnValidationException>(() => orderItem.UpdateQuantity(quantity));
    }

    [Fact]
    public void UpdatePrice_ValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var orderItem = new OrderItem(1L, 2, 10.0m);
        var newPrice = 15.0m;

        // Act
        orderItem.UpdatePrice(newPrice);

        // Assert
        Assert.Equal(newPrice, orderItem.Price);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void UpdatePrice_InvalidPrice_ShouldThrowOnValidationException(decimal price)
    {
        // Arrange
        var orderItem = new OrderItem(1L, 2, 10.0m);

        // Act & Assert
        Assert.Throws<OnValidationException>(() => orderItem.UpdatePrice(price));
    }

    [Theory]
    [InlineData(2, 10.0, 20.0)]
    [InlineData(3, 15.5, 46.5)]
    [InlineData(1, 99.99, 99.99)]
    public void GetSubTotal_DifferentValues_ShouldReturnCorrectSubTotal(int quantity, decimal price, decimal expectedSubTotal)
    {
        // Arrange
        var orderItem = new OrderItem(1L, quantity, price);

        // Act
        var subTotal = orderItem.GetSubTotal();

        // Assert
        Assert.Equal(expectedSubTotal, subTotal);
    }
}