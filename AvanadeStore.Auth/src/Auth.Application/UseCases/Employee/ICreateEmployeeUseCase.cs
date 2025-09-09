using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;

namespace Auth.Application.UseCases.Employee;
public interface ICreateEmployeeUseCase
{
    Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateEmployeeDTO request);
}
