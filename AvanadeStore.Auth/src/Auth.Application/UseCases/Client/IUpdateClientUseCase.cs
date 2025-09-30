using Auth.Application.DTOs.Requests;

namespace Auth.Application.UseCases.Client;

public interface IUpdateClientUseCase
{
    Task<bool> ExecuteAsync(Guid id, RequestUpdateClientDTO request);
}