using Inventory.Application.DTOs.Responses;
using Inventory.Application.UseCases.Product;
using Inventory.Domain.Interfaces;
using Inventory.Exception.CustomExceptions;
using Moq;

namespace Inventory.Tests.UseCases.Product;

public class GetProductUseCaseTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly GetProductUseCase _useCase;

    public GetProductUseCaseTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _useCase = new GetProductUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1L;
        var product = new Inventory.Domain.Entities.Product("Produto Teste", "Descrição", 99.99m, 10);

        // Use reflection to set the Id property since it's init-only
        var idProperty = typeof(Inventory.Domain.Entities.Product).GetProperty("Id");
        idProperty?.SetValue(product, productId);

        _repositoryMock.Setup(x => x.GetAsync(productId)).ReturnsAsync(product);

        // Act
        var result = await _useCase.ExecuteAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Description, result.Description);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(product.Stock, result.Stock);

        _repositoryMock.Verify(x => x.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingProduct_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var productId = 999L;
        _repositoryMock.Setup(x => x.GetAsync(productId)).ReturnsAsync((Inventory.Domain.Entities.Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() => _useCase.ExecuteAsync(productId));

        _repositoryMock.Verify(x => x.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetAllAsync_WithProducts_ShouldReturnPaginatedList()
    {
        // Arrange
        var products = new List<Inventory.Domain.Entities.Product>();
        for (int i = 1; i <= 15; i++)
        {
            var product = new Inventory.Domain.Entities.Product($"Produto {i}", $"Descrição {i}", i * 10m, i);
            var idProperty = typeof(Inventory.Domain.Entities.Product).GetProperty("Id");
            idProperty?.SetValue(product, (long)i);
            products.Add(product);
        }

        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _useCase.ExecuteGetAllAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.ResponseProducts.Count); // First page should have 10 items
        Assert.Equal(1, result.Page);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(2, result.TotalPages); // 15 items / 10 per page = 2 pages

        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetAllAsync_SecondPage_ShouldReturnCorrectItems()
    {
        // Arrange
        var products = new List<Inventory.Domain.Entities.Product>();
        for (int i = 1; i <= 15; i++)
        {
            var product = new Inventory.Domain.Entities.Product($"Produto {i}", $"Descrição {i}", i * 10m, i);
            var idProperty = typeof(Inventory.Domain.Entities.Product).GetProperty("Id");
            idProperty?.SetValue(product, (long)i);
            products.Add(product);
        }

        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _useCase.ExecuteGetAllAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.ResponseProducts.Count); // Second page should have 5 remaining items
        Assert.Equal(2, result.Page);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(2, result.TotalPages);

        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteGetAllAsync_EmptyList_ShouldReturnEmptyPaginatedList()
    {
        // Arrange
        var products = new List<Inventory.Domain.Entities.Product>();
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _useCase.ExecuteGetAllAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ResponseProducts);
        Assert.Equal(1, result.Page);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);

        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
}