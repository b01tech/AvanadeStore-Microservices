using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Application.UseCases.Employee;
using Auth.Domain.Enums;
using Auth.Domain.Interfaces;
using Auth.Exception.CustomExceptions;
using MapsterMapper;
using Moq;

namespace Auth.Tests.UseCases.Employee;
public class CreateEmployeeUseCaseTests
{
    private readonly Mock<IEncrypter> _encrypterMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly CreateEmployeeUseCase _useCase;

    public CreateEmployeeUseCaseTests()
    {
        _encrypterMock = new Mock<IEncrypter>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IEmployeeRepository>();
        _useCase = new CreateEmployeeUseCase(
            _encrypterMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _repositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldCreateEmployeeSuccessfully()
    {
        // Arrange
        var request = new RequestCreateEmployeeDTO(
            "Maria Silva",
            "maria@empresa.com",
            "password123",
            UserRole.Manager
        );
        var hashedPassword = "hashedPassword123";
        var employeeId = Guid.CreateVersion7();
        var createdAt = DateTime.UtcNow;

        var employee = new Auth.Domain.Entities.Employee(
            "Maria Silva",
            "maria@empresa.com",
            hashedPassword,
            UserRole.Manager
        );
        var expectedResponse = new ResponseCreateUserDTO(employeeId, "Maria Silva", createdAt);

        _encrypterMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);
        _mapperMock
            .Setup(x => x.Map<Auth.Domain.Entities.Employee>(It.IsAny<RequestCreateEmployeeDTO>()))
            .Returns(employee);
        _mapperMock.Setup(x => x.Map<ResponseCreateUserDTO>(employee)).Returns(expectedResponse);
        _repositoryMock.Setup(x => x.AddAsync(employee)).ReturnsAsync(employee);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
        _encrypterMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Auth.Domain.Entities.Employee>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "maria@empresa.com", "password123", UserRole.Manager)] // Nome vazio
    [InlineData("Maria Silva", "", "password123", UserRole.Manager)] // Email vazio
    [InlineData("Maria Silva", "maria@empresa.com", "123", UserRole.Manager)] // Senha muito curta
    [InlineData("Maria Silva", "email-invalido", "password123", UserRole.Manager)] // Email inválido
    [InlineData("Maria Silva", "maria@empresa.com", "password123", UserRole.Client)] // Role Client não permitido
    public async Task ExecuteAsync_InvalidRequest_ShouldThrowOnValidationException(
       string name,
       string email,
       string password,
       UserRole role
   )
    {
        // Arrange
        var request = new RequestCreateEmployeeDTO(name, email, password, role);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _useCase.ExecuteAsync(request)
        );
        Assert.NotEmpty(exception.ErrorMessages);

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Auth.Domain.Entities.Employee>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
