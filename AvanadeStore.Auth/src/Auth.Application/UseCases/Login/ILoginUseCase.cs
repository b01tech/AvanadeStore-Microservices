using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;

namespace Auth.Application.UseCases.Login;
public interface ILoginUseCase
{
    Task<ResponseUserToken> LoginByCpf(RequestLoginByCpfDTO request);
    Task<ResponseUserToken> LoginByEmail(RequestLoginByEmailDTO request);
}
