using Inventory.Domain.Entities;
using Inventory.Exception.CustomExceptions;

namespace Inventory.Tests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateProductSuccessfully()
    {
        // Arrange
        var name = "Produto Teste";
        var description = "Descrição do produto teste";
        var price = 99.99m;
        var stock = 10;

        // Act
        var product = new Product(name, description, price, stock);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
        Assert.Equal(price, product.Price);
        Assert.Equal(stock, product.Stock);
    }

    [Theory]
    [InlineData("", "Descrição", 10.0, 5)]
    [InlineData("   ", "Descrição", 10.0, 5)]
    [InlineData("Produto", "Descrição", -1.0, 5)]
    [InlineData("Produto", "Descrição", 10.0, -1)]
    public void Constructor_InvalidParameters_ShouldThrowOnValidationException(string name, string description, decimal price, int stock)
    {
        // Act & Assert
        Assert.Throws<OnValidationException>(() => new Product(name, description, price, stock));
    }

    [Fact]
    public void IncreaseStock_ValidQuantity_ShouldIncreaseStock()
    {
        // Arrange
        var product = new Product("Produto", "Descrição", 10.0m, 5);
        var quantityToAdd = 3;
        var expectedStock = 8;

        // Act
        product.IncreseStock(quantityToAdd);

        // Assert
        Assert.Equal(expectedStock, product.Stock);
    }

    [Fact]
    public void DecreaseStock_ValidQuantity_ShouldDecreaseStock()
    {
        // Arrange
        var product = new Product("Produto", "Descrição", 10.0m, 10);
        var quantityToRemove = 3;
        var expectedStock = 7;

        // Act
        product.DecreaseStock(quantityToRemove);

        // Assert
        Assert.Equal(expectedStock, product.Stock);
    }

    [Fact]
    public void DecreaseStock_QuantityGreaterThanStock_ShouldThrowOnValidationException()
    {
        // Arrange
        var product = new Product("Produto", "Descrição", 10.0m, 5);
        var quantityToRemove = 10;

        // Act & Assert
        Assert.Throws<OnValidationException>(() => product.DecreaseStock(quantityToRemove));
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product("Produto Original", "Descrição Original", 10.0m, 5);
        var newName = "Produto Atualizado";
        var newDescription = "Nova Descrição";
        var newPrice = 15.0m;
        var newStock = 8;

        // Act
        product.Update(newName, newDescription, newPrice, newStock);

        // Assert
        Assert.Equal(newName, product.Name);
        Assert.Equal(newDescription, product.Description);
        Assert.Equal(newPrice, product.Price);
        Assert.Equal(newStock, product.Stock);
    }

    [Theory]
    [InlineData("", "Descrição", 10.0, 5)]
    [InlineData("   ", "Descrição", 10.0, 5)]
    [InlineData("Produto", "Descrição", -1.0, 5)]
    [InlineData("Produto", "Descrição", 10.0, -1)]
    public void Update_InvalidParameters_ShouldThrowOnValidationException(string name, string description, decimal price, int stock)
    {
        // Arrange
        var product = new Product("Produto Original", "Descrição Original", 10.0m, 5);

        // Act & Assert
        Assert.Throws<OnValidationException>(() => product.Update(name, description, price, stock));
    }

    [Theory]
    [InlineData(5, 3, true)]
    [InlineData(5, 5, true)]
    [InlineData(5, 6, false)]
    public void IsStockAvailable_DifferentQuantities_ShouldReturnCorrectResult(int currentStock, int requestedQuantity, bool expectedResult)
    {
        // Arrange
        var product = new Product("Produto", "Descrição", 10.0m, currentStock);

        // Act
        var result = product.IsStockAvailable(requestedQuantity);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}