using Auth.Application.DTOs.Requests;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Application.UseCases.Client;

internal class UpdateClientUseCase : IUpdateClientUseCase
{
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClientUseCase(IClientRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(Guid id, RequestUpdateClientDTO request)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client == null)
            throw new NotFoundException(ResourceErrorMessages.CLIENT_NOT_FOUND);

        // Validar se email já existe para outro cliente
        var existingClientWithEmail = await _repository.ExistsByEmailAsync(request.Email);
        if (existingClientWithEmail && client.Email != request.Email)
            throw new OnValidationException(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);

        // Validar se CPF já existe para outro cliente
        var cpf = new Cpf(request.Cpf);
        var existingClientWithCpf = await _repository.ExistsByCpfAsync(cpf.Value);
        if (existingClientWithCpf && client.Cpf.Value != cpf.Value)
            throw new OnValidationException(ResourceErrorMessages.CPF_ALREADY_REGISTERED);

        // Atualizar dados do cliente
        client.UpdateName(request.Name);
        client.UpdateEmail(request.Email);
        client.UpdateCpf(cpf.Value);

        var result = await _repository.UpdateAsync(client);
        if (result)
            await _unitOfWork.CommitAsync();

        return result;
    }
}