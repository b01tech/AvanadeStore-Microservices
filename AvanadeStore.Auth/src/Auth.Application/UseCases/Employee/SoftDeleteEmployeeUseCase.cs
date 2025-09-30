using Auth.Domain.Enums;
using Auth.Domain.Interfaces;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Application.UseCases.Employee;

internal class SoftDeleteEmployeeUseCase : ISoftDeleteEmployeeUseCase
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteEmployeeUseCase(IEmployeeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(Guid id, Guid requestingUserId)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null)
            throw new NotFoundException(ResourceErrorMessages.EMPLOYEE_NOT_FOUND);

        // Verificar se o usuário que está fazendo a requisição é um Manager
        var requestingEmployee = await _repository.GetByIdAsync(requestingUserId);
        if (requestingEmployee == null || requestingEmployee.Role != UserRole.Manager)
            throw new UnauthorizedException(ResourceErrorMessages.UNAUTHORIZED_ACCESS);

        var result = await _repository.SoftDeleteAsync(id);
        if (result)
            await _unitOfWork.CommitAsync();

        return result;
    }
}