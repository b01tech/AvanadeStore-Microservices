using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Application.Services.Token;
using Auth.Application.UseCases.Login;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using Moq;

namespace Auth.Tests.UseCases.Login;
public class LoginUseCaseTests
{
    private readonly Mock<IEncrypter> _encrypterMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _encrypterMock = new Mock<IEncrypter>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _useCase = new LoginUseCase(
            _encrypterMock.Object,
            _clientRepositoryMock.Object,
            _tokenServiceMock.Object,
            _employeeRepositoryMock.Object);
    }

    [Fact]
    public async Task LoginByCpf_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new RequestLoginByCpfDTO("11144477735", "password123");
        var hashedPassword = "hashedPassword123";
        var accessToken = "jwt-token-123";
        var client = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", hashedPassword, new Cpf("11144477735"));
        var expectedResponse = new ResponseUserToken(client.Name, DateTime.UtcNow.AddHours(1), accessToken);

        _encrypterMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
        _clientRepositoryMock.Setup(x => x.GetByCpfAndPasswordAsync("111.444.777-35", hashedPassword)).ReturnsAsync(client);
        _tokenServiceMock.Setup(x => x.GenereteAccessToken(client)).Returns(accessToken);

        // Act
        var result = await _useCase.LoginByCpf(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Username, client.Name);
        Assert.Equal(result.AccessToken,accessToken);
        
        _encrypterMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        _clientRepositoryMock.Verify(x => x.GetByCpfAndPasswordAsync("111.444.777-35", hashedPassword), Times.Once);
        _tokenServiceMock.Verify(x => x.GenereteAccessToken(client), Times.Once);
    }

    [Fact]
    public async Task LoginByEmail_ValidClientCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new RequestLoginByEmailDTO("joao@email.com", "password123");
        var hashedPassword = "hashedPassword123";
        var accessToken = "jwt-token-123";
        var client = new Auth.Domain.Entities.Client("João Silva", "joao@email.com", hashedPassword, new Cpf("11144477735"));

        _encrypterMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
        _clientRepositoryMock.Setup(x => x.GetByEmailAndPasswordAsync(request.Email, hashedPassword)).ReturnsAsync(client);
        _tokenServiceMock.Setup(x => x.GenereteAccessToken(client)).Returns(accessToken);

        // Act
        var result = await _useCase.LoginByEmail(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Username, client.Name);
        Assert.Equal(result.AccessToken, accessToken);

        _encrypterMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        _clientRepositoryMock.Verify(x => x.GetByEmailAndPasswordAsync(request.Email, hashedPassword), Times.Once);
        _employeeRepositoryMock.Verify(x => x.GetByEmailAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenereteAccessToken(client), Times.Once);
    }

    [Fact]
    public async Task LoginByEmail_InvalidCredentials_ShouldThrowNotAuthorizedException()
    {
        // Arrange
        var request = new RequestLoginByEmailDTO("invalid@email.com", "wrongPassword");
        var hashedPassword = "hashedWrongPassword";

        _encrypterMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
        _clientRepositoryMock.Setup(x => x.GetByEmailAndPasswordAsync(request.Email, hashedPassword)).ReturnsAsync((Auth.Domain.Entities.Client?)null);
        _employeeRepositoryMock.Setup(x => x.GetByEmailAndPasswordAsync(request.Email, hashedPassword)).ReturnsAsync((Auth.Domain.Entities.Employee?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotAuthorizedException>(() => _useCase.LoginByEmail(request));
        Assert.Equal(exception.Message, ResourceErrorMessages.LOGIN_FAIL);

        _encrypterMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        _clientRepositoryMock.Verify(x => x.GetByEmailAndPasswordAsync(request.Email, hashedPassword), Times.Once);
        _employeeRepositoryMock.Verify(x => x.GetByEmailAndPasswordAsync(request.Email, hashedPassword), Times.Once);
        _tokenServiceMock.Verify(x => x.GenereteAccessToken(It.IsAny<User>()), Times.Never);
    }
    [Fact]
    public async Task LoginByCpf_InvalidCredentials_ShouldThrowNotAuthorizedException()
    {
        // Arrange
        var request = new RequestLoginByCpfDTO("11144477735", "wrongPassword");
        var hashedPassword = "hashedWrongPassword";

        _encrypterMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
        _clientRepositoryMock.Setup(x => x.GetByCpfAndPasswordAsync("111.444.777-35", hashedPassword)).ReturnsAsync((Auth.Domain.Entities.Client?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotAuthorizedException>(() => _useCase.LoginByCpf(request));
        Assert.Equal(exception.Message, ResourceErrorMessages.LOGIN_FAIL);

        _encrypterMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        _clientRepositoryMock.Verify(x => x.GetByCpfAndPasswordAsync("111.444.777-35", hashedPassword), Times.Once);
        _tokenServiceMock.Verify(x => x.GenereteAccessToken(It.IsAny<User>()), Times.Never);
    }
}
