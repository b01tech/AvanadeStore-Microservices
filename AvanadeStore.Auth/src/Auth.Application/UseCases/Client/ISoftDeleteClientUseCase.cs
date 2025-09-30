namespace Auth.Application.UseCases.Client;

public interface ISoftDeleteClientUseCase
{
    Task<bool> ExecuteAsync(Guid id);
}