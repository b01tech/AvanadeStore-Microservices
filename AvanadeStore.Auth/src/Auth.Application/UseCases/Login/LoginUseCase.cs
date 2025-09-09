using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Application.Services.Token;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Application.UseCases.Login;
internal class LoginUseCase : ILoginUseCase
{
    private readonly IEncrypter _encrypter;
    private readonly IClientRepository _clientRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ITokenService _tokenService;

    public LoginUseCase(IEncrypter encrypter, IClientRepository clientRepository, ITokenService tokenService, IEmployeeRepository employeeRepository)
    {
        _encrypter = encrypter;
        _clientRepository = clientRepository;
        _tokenService = tokenService;
        _employeeRepository = employeeRepository;
    }

    public async Task<ResponseUserToken> LoginByCpf(RequestLoginByCpfDTO request)
    {
        ValidateInputs(request.Cpf, request.Password);
        var cpf = new Cpf(request.Cpf);
        var passwordHashed = _encrypter.Encrypt(request.Password);

        var client = await _clientRepository.GetByCpfAndPasswordAsync(cpf.Value, passwordHashed) ?? throw new NotAuthorizedException(ResourceErrorMessages.LOGIN_FAIL);
        var accessToken = _tokenService.GenereteAccessToken(client);
        return new ResponseUserToken(client.Name, DateTime.UtcNow.AddHours(1), accessToken);
    }

    public async Task<ResponseUserToken> LoginByEmail(RequestLoginByEmailDTO request)
    {
        ValidateInputs(request.Email, request.Password);
        var passwordHashed = _encrypter.Encrypt(request.Password);
        User? user = await _clientRepository.GetByEmailAndPasswordAsync(request.Email, passwordHashed);
        if (user is null)
            user = await _employeeRepository.GetByEmailAndPasswordAsync(request.Email, passwordHashed);
        if(user is null)
            throw new NotAuthorizedException(ResourceErrorMessages.LOGIN_FAIL);

        var accessToken = _tokenService.GenereteAccessToken(user);
        return new ResponseUserToken(user.Name, DateTime.UtcNow.AddHours(1), accessToken);
    }

    private static void ValidateInputs(string value1, string value2)
    {
        if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2))
        {
            throw new NotAuthorizedException(ResourceErrorMessages.LOGIN_FAIL);
        }
    }
}
