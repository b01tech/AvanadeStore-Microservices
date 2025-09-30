using Auth.Application.DTOs.Requests;
using Auth.Application.UseCases.Employee;
using Auth.Domain.Enums;
using Auth.Domain.Interfaces;
using Moq;

namespace Auth.Tests.UseCases.Employee;
public class UpdateEmployeeUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateEmployeeUseCase _useCase;

    public UpdateEmployeeUseCaseTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new UpdateEmployeeUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldUpdateEmployeeSuccessfully()
    {
        // Arrange
        var employeeId = Guid.CreateVersion7();
        var request = new RequestUpdateEmployeeDTO("Maria Silva Atualizada", "maria.nova@empresa.com", UserRole.Manager);

        var existingEmployee = new Auth.Domain.Entities.Employee("Maria Silva", "maria@empresa.com", "hashedPassword", UserRole.Manager);

        _repositoryMock.Setup(x => x.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.UpdateAsync(existingEmployee)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(employeeId, request);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
        _repositoryMock.Verify(x => x.ExistsByEmailAsync(request.Email), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(existingEmployee), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UpdateFails_ShouldReturnFalseAndNotCommit()
    {
        // Arrange
        var employeeId = Guid.CreateVersion7();
        var request = new RequestUpdateEmployeeDTO("Maria Silva", "maria@empresa.com", UserRole.Manager);

        var existingEmployee = new Auth.Domain.Entities.Employee("Maria Silva", "maria@empresa.com", "hashedPassword", UserRole.Manager);

        _repositoryMock.Setup(x => x.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.UpdateAsync(existingEmployee)).ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(employeeId, request);

        // Assert
        Assert.False(result); 
        _repositoryMock.Verify(x => x.UpdateAsync(existingEmployee), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
