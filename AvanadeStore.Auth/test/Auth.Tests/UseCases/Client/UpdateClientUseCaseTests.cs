using Auth.Application.DTOs.Requests;
using Auth.Application.UseCases.Client;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using Moq;

namespace Auth.Tests.UseCases.Client;
public class UpdateClientUseCaseTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateClientUseCase _useCase;

    public UpdateClientUseCaseTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new UpdateClientUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldUpdateClientSuccessfully()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var request = new RequestUpdateClientDTO("João Silva Atualizado", "joao.novo@email.com", "98765432100");

        var existingClient = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", "hashedPassword", new Cpf("11144477735"));

        _repositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(It.IsAny<string>())).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.UpdateAsync(existingClient)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(clientId, request);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(x => x.GetByIdAsync(clientId), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByCpfAsync(It.IsAny<string>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(existingClient), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmailAlreadyExistsForAnotherClient_ShouldThrowOnValidationException()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var request = new RequestUpdateClientDTO("João Silva", "outro@email.com", "11144477735");

        var existingClient = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", "hashedPassword", new Cpf("11144477735"));

        _repositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(() => _useCase.ExecuteAsync(clientId, request));
        Assert.Contains(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED, exception.ErrorMessages);

        _repositoryMock.Verify(x => x.GetByIdAsync(clientId), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Auth.Domain.Entities.Client>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_UpdateFails_ShouldReturnFalseAndNotCommit()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var request = new RequestUpdateClientDTO("João Silva", "joao@email.com", "11144477735");

        var existingClient = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", "hashedPassword", new Cpf("11144477735"));

        _repositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(It.IsAny<string>())).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.UpdateAsync(existingClient)).ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(clientId, request);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(x => x.UpdateAsync(existingClient), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
