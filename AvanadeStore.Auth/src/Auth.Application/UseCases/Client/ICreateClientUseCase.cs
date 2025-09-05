using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;

namespace Auth.Application.UseCases.Client;
public interface ICreateClientUseCase
{
    Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateClientDTO request);
}
