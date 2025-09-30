using Auth.Application.DTOs.Requests;

namespace Auth.Application.UseCases.Employee;

public interface IUpdateEmployeeUseCase
{
    Task<bool> ExecuteAsync(Guid id, RequestUpdateEmployeeDTO request);
}