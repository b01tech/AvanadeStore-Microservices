using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Services.Criptography;
internal class Encrypter : IEncrypter
{
    private readonly string _secretKey;

    public Encrypter(IConfiguration config)
    {
        _secretKey = config["Settings:SecretKey"] ?? throw new LoadEnvSettingsException(ResourceErrorMessages.SECRETKEY_ERROR);
    }

    public string Encrypt(string input)
    {
        var password = $"{input}{_secretKey}";
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashedBytes = SHA256.HashData(passwordBytes);
        return Convert.ToBase64String(hashedBytes);
    }
    public bool Compare(string plainText, string hashedText)
    {
        return Encrypt(plainText).Equals(hashedText);
    }

}
