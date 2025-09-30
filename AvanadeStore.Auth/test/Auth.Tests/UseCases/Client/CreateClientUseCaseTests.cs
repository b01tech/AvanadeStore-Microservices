using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Application.UseCases.Client;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using MapsterMapper;
using Moq;

namespace Auth.Tests.UseCases.Client;
public class CreateClientUseCaseTests
{
    private readonly Mock<IEncrypter> _encrypterMock;
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateClientUseCase _useCase;

    public CreateClientUseCaseTests()
    {
        _encrypterMock = new Mock<IEncrypter>();
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _useCase = new CreateClientUseCase(
            _encrypterMock.Object,
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldCreateClientSuccessfully()
    {
        // Arrange
        var request = new RequestCreateClientDTO(
            "João Silva",
            "joao@email.com",
            "11144477735",
            "password123"
        );
        var hashedPassword = "hashedPassword123";
        var clientId = Guid.CreateVersion7();
        var createdAt = DateTime.UtcNow;

        var client = new Auth.Domain.Entities.Client(
            "João Silva",
            "joao@email.com",
            hashedPassword,
            new Cpf("11144477735")
        );
        var expectedResponse = new ResponseCreateUserDTO(clientId, "João Silva", createdAt);

        _encrypterMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mapperMock
            .Setup(x => x.Map<Auth.Domain.Entities.Client>(It.IsAny<RequestCreateClientDTO>()))
            .Returns(client);
        _mapperMock.Setup(x => x.Map<ResponseCreateUserDTO>(client)).Returns(expectedResponse);
        _repositoryMock.Setup(x => x.AddAsync(client)).ReturnsAsync(client);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
        Assert.Equal(expectedResponse.CreateAt, result.CreateAt);

        _encrypterMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByCpfAsync(It.IsAny<string>()), Times.Once);
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Auth.Domain.Entities.Client>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmailAlreadyExists_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = new RequestCreateClientDTO(
            "João Silva",
            "joao@email.com",
            "11144477735",
            "password123"
        );

        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _useCase.ExecuteAsync(request)
        );
        Assert.Contains(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED, exception.ErrorMessages);

        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Auth.Domain.Entities.Client>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CpfAlreadyExists_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = new RequestCreateClientDTO(
            "João Silva",
            "joao@email.com",
            "11144477735",
            "password123"
        );

        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _useCase.ExecuteAsync(request)
        );
        Assert.Contains(ResourceErrorMessages.CPF_ALREADY_REGISTERED, exception.ErrorMessages);

        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByCpfAsync(It.IsAny<string>()), Times.Once);
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Auth.Domain.Entities.Client>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData("", "joao@email.com", "11144477735", "password123")] // Nome vazio
    [InlineData("João Silva", "", "11144477735", "password123")] // Email vazio
    [InlineData("João Silva", "joao@email.com", "invalid-cpf", "password123")] // CPF inválido
    [InlineData("João Silva", "joao@email.com", "11144477735", "123")] // Senha muito curta
    [InlineData("João Silva", "email-invalido", "11144477735", "password123")] // Email inválido
    [InlineData("João Silva", "joao@email.com", "123", "password123")] // CPF inválido
    public async Task ExecuteAsync_InvalidRequest_ShouldThrowOnValidationException(
       string name,
       string email,
       string cpf,
       string password
   )
    {
        // Arrange
        var request = new RequestCreateClientDTO(name, email, cpf, password);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _useCase.ExecuteAsync(request)
        );
        Assert.NotEmpty(exception.ErrorMessages);

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Auth.Domain.Entities.Client>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
