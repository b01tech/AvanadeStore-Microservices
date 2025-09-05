using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Exception.CustomExceptions;

namespace Auth.Application.UseCases.Client;
internal class CreateClientUseCase : ICreateClientUseCase
{
    private readonly IEncrypter _encrypter;

    public CreateClientUseCase(IEncrypter encrypter)
    {
        _encrypter = encrypter;
    }

    public async Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateClientDTO request)
    {
        await ValidateAsync(request);
        var hashedPassword = _encrypter.Encrypt(request.Password);
        var result = new ResponseCreateUserDTO(Guid.CreateVersion7(), request.Name, DateTime.UtcNow);
        return await Task.FromResult(result);
    }

    private static async Task ValidateAsync(RequestCreateClientDTO request)
    {
        var validator = new CreateClientValidator();
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new OnValidationException(errorMessages);
        }          
    }
}
