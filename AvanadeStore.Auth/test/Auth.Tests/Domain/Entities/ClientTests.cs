using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;

namespace Auth.Tests.Domain.Entities;
public class ClientTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateClientSuccessfully()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao@email.com";
        var passwordHash = "hashedPassword123";
        var cpf = new Cpf("11144477735");

        // Act
        var client = new Client(name, email, passwordHash, cpf);

        // Assert
        Assert.NotNull(client);
        Assert.NotEqual(Guid.Empty, client.Id);
        Assert.Equal(name, client.Name);
        Assert.Equal(email, client.Email);
        Assert.Equal(passwordHash, client.PasswordHash);
        Assert.Equal(UserRole.Client, client.Role);
        Assert.Equal(cpf, client.Cpf);
        Assert.True(client.IsActive);
        Assert.True(DateTime.UtcNow.Subtract(client.CreatedAt).Duration() < TimeSpan.FromSeconds(1));
        Assert.True(DateTime.UtcNow.Subtract(client.UpdatedAt).Duration() < TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "joao@email.com", "hashedPassword123")]
    [InlineData("   ", "joao@email.com", "hashedPassword123")]
    [InlineData("João Silva", "", "hashedPassword123")]
    [InlineData("João Silva", "   ", "hashedPassword123")]
    [InlineData("João Silva", "joao@email.com", "")]
    [InlineData("João Silva", "joao@email.com", "   ")]
    public void Constructor_InvalidParameters_ShouldThrowOnValidationException(string name, string email, string passwordHash)
    {
        // Arrange
        var cpf = new Cpf("11144477735");

        // Act & Assert
        var exception = Assert.Throws<OnValidationException>(() => new Client(name, email, passwordHash, cpf));
        Assert.NotEmpty(exception.ErrorMessages);
    }

    [Fact]
    public void UpdateName_ValidName_ShouldUpdateNameAndUpdatedAt()
    {
        // Arrange
        var client = new Client("João Silva", "joao@email.com", "hashedPassword123", new Cpf("11144477735"));
        var originalUpdatedAt = client.UpdatedAt;
        var newName = "João Silva Santos";

        //Delay
        Thread.Sleep(10);

        // Act
        client.UpdateName(newName);

        // Assert
        Assert.Equal(client.Name, newName);
        Assert.True(client.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndUpdateUpdatedAt()
    {
        // Arrange
        var client = new Client("João Silva", "joao@email.com", "hashedPassword123", new Cpf("11144477735"));
        var originalUpdatedAt = client.UpdatedAt;

        // Wait a small amount to ensure UpdatedAt changes
        Thread.Sleep(10);

        // Act
        client.Deactivate();

        // Assert
        Assert.False(client.IsActive);
        Assert.True(client.UpdatedAt > originalUpdatedAt);
    }
}
