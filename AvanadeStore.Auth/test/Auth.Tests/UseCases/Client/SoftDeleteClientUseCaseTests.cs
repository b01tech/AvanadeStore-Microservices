using Auth.Application.UseCases.Client;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Moq;

namespace Auth.Tests.UseCases.Client;
public class SoftDeleteClientUseCaseTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SoftDeleteClientUseCase _useCase;

    public SoftDeleteClientUseCaseTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new SoftDeleteClientUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidClientId_ShouldSoftDeleteSuccessfully()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var existingClient = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", "hashedPassword", new Cpf("11144477735"));

        _repositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
        _repositoryMock.Setup(x => x.SoftDeleteAsync(clientId)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(clientId);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(x => x.GetByIdAsync(clientId), Times.Once);
        _repositoryMock.Verify(x => x.SoftDeleteAsync(clientId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SoftDeleteFails_ShouldReturnFalseAndNotCommit()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var existingClient = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", "hashedPassword", new Cpf("11144477735"));

        _repositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
        _repositoryMock.Setup(x => x.SoftDeleteAsync(clientId)).ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(clientId);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(x => x.GetByIdAsync(clientId), Times.Once);
        _repositoryMock.Verify(x => x.SoftDeleteAsync(clientId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
