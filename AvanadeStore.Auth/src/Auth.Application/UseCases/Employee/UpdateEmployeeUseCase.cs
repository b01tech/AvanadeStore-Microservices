using Auth.Application.DTOs.Requests;
using Auth.Domain.Interfaces;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Application.UseCases.Employee;

internal class UpdateEmployeeUseCase : IUpdateEmployeeUseCase
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeUseCase(IEmployeeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(Guid id, RequestUpdateEmployeeDTO request)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null)
            throw new NotFoundException(ResourceErrorMessages.EMPLOYEE_NOT_FOUND);

        // Validar se email já existe para outro funcionário
        var existingEmployeeWithEmail = await _repository.ExistsByEmailAsync(request.Email);
        if (existingEmployeeWithEmail && employee.Email != request.Email)
            throw new OnValidationException(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);

        // Atualizar dados do funcionário
        employee.UpdateName(request.Name);
        employee.UpdateEmail(request.Email);
        employee.UpdateRole(request.Role);

        var result = await _repository.UpdateAsync(employee);
        if (result)
            await _unitOfWork.CommitAsync();

        return result;
    }
}