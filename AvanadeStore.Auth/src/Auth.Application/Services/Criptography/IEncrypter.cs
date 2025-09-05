namespace Auth.Application.Services.Criptography;
public interface IEncrypter
{
    string Encrypt(string input);
    bool Compare(string plainText, string hashedText);
}
