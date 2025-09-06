using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Application.UseCases.Login;
internal class LoginUseCase : ILoginUseCase
{
    private readonly IEncrypter _encrypter;
    private readonly IClientRepository _repository;

    public LoginUseCase(IEncrypter encrypter, IClientRepository repository)
    {
        _encrypter = encrypter;
        _repository = repository;
    }

    public async Task<ResponseUserToken> LoginByCpf(RequestLoginByCpfDTO request)
    {
        ValidateInputs(request.Cpf, request.Password);
        var cpf = new Cpf(request.Cpf);
        var passwordHashed = _encrypter.Encrypt(request.Password);

        var client = await _repository.GetByCpfAndPasswordAsync(cpf.Value, passwordHashed) ?? throw new NotAuthorizedException(ResourceErrorMessages.LOGIN_FAIL);
        return new ResponseUserToken(client.Name, DateTime.UtcNow.AddHours(1), "token de teste");
    }

    public async Task<ResponseUserToken> LoginByEmail(RequestLoginByEmailDTO request)
    {
        ValidateInputs(request.Email, request.Password);
        var passwordHashed = _encrypter.Encrypt(request.Password);
        var client = await _repository.GetByEmailAndPasswordAsync(request.Email, passwordHashed) ?? throw new NotAuthorizedException(ResourceErrorMessages.LOGIN_FAIL);
        return new ResponseUserToken(client.Name, DateTime.UtcNow.AddHours(1), "token de teste");
    }

    private static void ValidateInputs(string value1, string value2)
    {
        if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2))
        {
            throw new NotAuthorizedException(ResourceErrorMessages.LOGIN_FAIL);
        }
    }
}
