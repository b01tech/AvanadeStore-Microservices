namespace Auth.Application.UseCases.Employee;

public interface ISoftDeleteEmployeeUseCase
{
    Task<bool> ExecuteAsync(Guid id, Guid requestingUserId);
}