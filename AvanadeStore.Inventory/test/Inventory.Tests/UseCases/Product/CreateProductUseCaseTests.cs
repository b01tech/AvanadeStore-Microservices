using Inventory.Application.DTOs.Requests;
using Inventory.Application.DTOs.Responses;
using Inventory.Application.UseCases.Product;
using Inventory.Domain.Interfaces;
using Inventory.Exception.CustomExceptions;
using Moq;

namespace Inventory.Tests.UseCases.Product;

public class CreateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProductUseCase _useCase;

    public CreateProductUseCaseTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new CreateProductUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldCreateProductSuccessfully()
    {
        // Arrange
        var request = new RequestCreateProductDTO("Produto Teste", "Descrição do produto", 99.99m, 10);
        var productId = 1L;
        var product = new Inventory.Domain.Entities.Product("Produto Teste", "Descrição do produto", 99.99m, 10);

        // Use reflection to set the Id property since it's init-only
        var idProperty = typeof(Inventory.Domain.Entities.Product).GetProperty("Id");
        idProperty?.SetValue(product, productId);

        _repositoryMock.Setup(x => x.GetByName(request.Name)).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Inventory.Domain.Entities.Product>())).ReturnsAsync(product);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Price, result.Price);
        Assert.Equal(request.Stock, result.Stock);

        _repositoryMock.Verify(x => x.GetByName(request.Name), Times.Once);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Inventory.Domain.Entities.Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "Descrição", 10.0, 5)]
    [InlineData("   ", "Descrição", 10.0, 5)]
    public async Task ExecuteAsync_EmptyOrWhiteSpaceName_ShouldThrowInvalidArgumentsException(string name, string description, decimal price, int stock)
    {
        // Arrange
        var request = new RequestCreateProductDTO(name, description, price, stock);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidArgumentsException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvalidPrice_ShouldThrowInvalidArgumentsException()
    {
        // Arrange
        var request = new RequestCreateProductDTO("Produto", "Descrição", 0m, 5);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidArgumentsException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_NegativeStock_ShouldThrowInvalidArgumentsException()
    {
        // Arrange
        var request = new RequestCreateProductDTO("Produto", "Descrição", 10.0m, -1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidArgumentsException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ProductAlreadyExists_ShouldThrowAlreadyExistsException()
    {
        // Arrange
        var request = new RequestCreateProductDTO("Produto Existente", "Descrição", 10.0m, 5);

        _repositoryMock.Setup(x => x.GetByName(request.Name)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(() => _useCase.ExecuteAsync(request));

        _repositoryMock.Verify(x => x.GetByName(request.Name), Times.Once);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Inventory.Domain.Entities.Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}