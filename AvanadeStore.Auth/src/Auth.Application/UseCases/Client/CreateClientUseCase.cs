using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;

namespace Auth.Application.UseCases.Client;
internal class CreateClientUseCase : ICreateClientUseCase
{
    public Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateClientDTO request)
    {
        var result = new ResponseCreateUserDTO(Guid.CreateVersion7(), request.Name, DateTime.UtcNow);
        return Task.FromResult(result);
    }
}
