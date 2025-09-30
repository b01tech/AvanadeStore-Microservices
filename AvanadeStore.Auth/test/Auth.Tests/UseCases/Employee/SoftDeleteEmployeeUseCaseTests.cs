using Auth.Application.UseCases.Employee;
using Auth.Domain.Enums;
using Auth.Domain.Interfaces;
using Moq;

namespace Auth.Tests.UseCases.Employee;
public class SoftDeleteEmployeeUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SoftDeleteEmployeeUseCase _useCase;

    public SoftDeleteEmployeeUseCaseTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new SoftDeleteEmployeeUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequestWithManagerRole_ShouldSoftDeleteEmployeeSuccessfully()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();

        var employeeToDelete = new Auth.Domain.Entities.Employee("João Silva", "joao@empresa.com", "hashedPassword", UserRole.Employee);
        var requestingEmployee = new Auth.Domain.Entities.Employee("Maria Manager", "maria@empresa.com", "hashedPassword", UserRole.Manager);

        _repositoryMock.SetupSequence(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(employeeToDelete)
            .ReturnsAsync(requestingEmployee);
        _repositoryMock.Setup(x => x.SoftDeleteAsync(employeeId)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(employeeId, requestingUserId);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
        _repositoryMock.Verify(x => x.GetByIdAsync(requestingUserId), Times.Once);
        _repositoryMock.Verify(x => x.SoftDeleteAsync(employeeId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SoftDeleteFails_ShouldReturnFalseAndNotCommit()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();

        var employeeToDelete = new Auth.Domain.Entities.Employee("João Silva", "joao@empresa.com", "hashedPassword", UserRole.Employee);
        var requestingEmployee = new Auth.Domain.Entities.Employee("Maria Manager", "maria@empresa.com", "hashedPassword", UserRole.Manager);

        _repositoryMock.SetupSequence(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(employeeToDelete)
            .ReturnsAsync(requestingEmployee);
        _repositoryMock.Setup(x => x.SoftDeleteAsync(employeeId)).ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(employeeId, requestingUserId);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(x => x.SoftDeleteAsync(employeeId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
