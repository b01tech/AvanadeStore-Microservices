using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Tests.Domain.ValueObjects;
public class CpfTests
{
    [Theory]
    [InlineData("11144477735", "111.444.777-35")]
    [InlineData("98765432100", "987.654.321-00")]
    public void Constructor_ValidCpf_ShouldCreateCpfWithFormattedValue(string input, string expectedFormatted)
    {
        // Act
        var cpf = new Cpf(input);

        // Assert
        Assert.NotNull(cpf);
        Assert.Equal(expectedFormatted, cpf.Value);
    }

    [Theory]
    [InlineData("987.654.321-00", "987.654.321-00")]
    [InlineData("111.444.777-35", "111.444.777-35")]
    public void Constructor_AlreadyFormattedValidCpf_ShouldCreateCpfSuccessfully(string input, string expected)
    {
        // Act
        var cpf = new Cpf(input);

        // Assert
        Assert.NotNull(cpf);
        Assert.Equal(expected, cpf.Value);
    }

    [Theory]
    [InlineData("12345678900")] // Invalid check digits
    [InlineData("11111111111")] // All same digits
    [InlineData("00000000000")] // All zeros
    [InlineData("22222222222")] // All same digits
    [InlineData("1234567890")]  // Only 10 digits
    [InlineData("123456789012")] // 12 digits
    [InlineData("")]            // Empty
    [InlineData("   ")]         // Whitespace
    [InlineData("abcdefghijk")] // Letters
    [InlineData("123.456.789-00")] // Invalid formatted CPF
    public void Constructor_InvalidCpf_ShouldThrowOnValidationException(string invalidCpf)
    {
        // Act & Assert
        var exception = Assert.Throws<OnValidationException>(() => new Cpf(invalidCpf));
        Assert.Contains(exception.Message, ResourceErrorMessages.CPF_INVALID);
    }

    [Fact]
    public void Constructor_NullCpf_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Cpf(null!));
    }

    [Theory]
    [InlineData("987.654.321-00")]
    [InlineData("111.444.777-35")]
    public void ToString_ShouldReturnFormattedValue(string expectedValue)
    {
        // Arrange
        var cpf = new Cpf(expectedValue);

        // Act
        var result = cpf.ToString();

        // Assert
        Assert.Equal(result, expectedValue);
    }
}
