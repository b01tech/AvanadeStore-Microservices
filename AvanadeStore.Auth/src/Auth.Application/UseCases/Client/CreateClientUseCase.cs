using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Exception.CustomExceptions;

namespace Auth.Application.UseCases.Client;
internal class CreateClientUseCase : ICreateClientUseCase
{
    public async Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateClientDTO request)
    {
        await ValidateAsync(request);
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
