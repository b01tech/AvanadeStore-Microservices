using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using Xunit;

namespace Auth.Tests.Domain.Entities;
public class EmployeeTests
{
    [Theory]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Employee)]
    public void Constructor_ValidRoles_ShouldCreateEmployeeSuccessfully(UserRole role)
    {
        // Arrange
        var name = "Maria Silva";
        var email = "maria@empresa.com";
        var passwordHash = "hashedPassword123";

        // Act
        var employee = new Employee(name, email, passwordHash, role);

        // Assert
        Assert.NotNull(employee);
        Assert.NotEqual(Guid.Empty, employee.Id);
        Assert.Equal(name, employee.Name);
        Assert.Equal(email, employee.Email);
        Assert.Equal(passwordHash, employee.PasswordHash);
        Assert.Equal(role, employee.Role);
        Assert.True(employee.IsActive);
        Assert.InRange(employee.CreatedAt, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        Assert.InRange(employee.UpdatedAt, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Constructor_ClientRole_ShouldThrowOnValidationException()
    {
        // Arrange
        var name = "Maria Silva";
        var email = "maria@empresa.com";
        var passwordHash = "hashedPassword123";
        var role = UserRole.Client;

        // Act & Assert
        var exception = Assert.Throws<OnValidationException>(() => new Employee(name, email, passwordHash, role));
        Assert.Contains(ResourceErrorMessages.EMPLOYEE_ROLE_INVALID, exception.ErrorMessages);
    }

    [Theory]
    [InlineData("", "maria@empresa.com", "hashedPassword123", UserRole.Manager)]
    [InlineData("   ", "maria@empresa.com", "hashedPassword123", UserRole.Manager)]
    [InlineData("Maria Silva", "", "hashedPassword123", UserRole.Manager)]
    [InlineData("Maria Silva", "   ", "hashedPassword123", UserRole.Manager)]
    [InlineData("Maria Silva", "maria@empresa.com", "", UserRole.Manager)]
    [InlineData("Maria Silva", "maria@empresa.com", "   ", UserRole.Manager)]
    public void Constructor_InvalidParameters_ShouldThrowOnValidationException(string name, string email, string passwordHash, UserRole role)
    {
        // Act & Assert
        var exception = Assert.Throws<OnValidationException>(() => new Employee(name, email, passwordHash, role));
        Assert.NotEmpty(exception.ErrorMessages);
    }

    [Theory]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Employee)]
    public void UpdateRole_ValidRoles_ShouldUpdateRoleAndUpdatedAt(UserRole newRole)
    {
        // Arrange
        var employee = new Employee("Maria Silva", "maria@empresa.com", "hashedPassword123", UserRole.Manager);
        var originalUpdatedAt = employee.UpdatedAt;

        // Wait a small amount to ensure UpdatedAt changes
        Thread.Sleep(10);

        // Act
        employee.UpdateRole(newRole);

        // Assert
        Assert.Equal(employee.Role, newRole);
        Assert.True(employee.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndUpdateUpdatedAt()
    {
        // Arrange
        var employee = new Employee("Maria Silva", "maria@empresa.com", "hashedPassword123", UserRole.Manager);
        var originalUpdatedAt = employee.UpdatedAt;

        // Delay
        Thread.Sleep(10);

        // Act
        employee.Deactivate();

        // Assert
        Assert.False(employee.IsActive);
        Assert.True(employee.UpdatedAt > originalUpdatedAt);
    }
}
