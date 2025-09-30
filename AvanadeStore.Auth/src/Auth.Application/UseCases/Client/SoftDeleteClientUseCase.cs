using Auth.Domain.Interfaces;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Application.UseCases.Client;

internal class SoftDeleteClientUseCase : ISoftDeleteClientUseCase
{
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteClientUseCase(IClientRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(Guid id)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client == null)
            throw new NotFoundException(ResourceErrorMessages.CLIENT_NOT_FOUND);

        var result = await _repository.SoftDeleteAsync(id);
        if (result)
            await _unitOfWork.CommitAsync();

        return result;
    }
}