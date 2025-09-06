using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Domain.Interfaces;
using Auth.Domain.ValueObjects;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using MapsterMapper;

namespace Auth.Application.UseCases.Client;
internal class CreateClientUseCase : ICreateClientUseCase
{
    private readonly IEncrypter _encrypter;
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateClientUseCase(IEncrypter encrypter, IClientRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _encrypter = encrypter;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateClientDTO request)
    {
        await ValidateAsync(request);
        var hashedPassword = _encrypter.Encrypt(request.Password);
        var client = _mapper.Map<Domain.Entities.Client>(request.WithHashedPassword(hashedPassword));

        await _repository.AddAsync(client);
        await _unitOfWork.CommitAsync();

        var response = _mapper.Map<ResponseCreateUserDTO>(client);
        return await Task.FromResult(response);
    }

    private async Task ValidateAsync(RequestCreateClientDTO request)
    {
        var validator = new CreateClientValidator();
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new OnValidationException(errorMessages);
        }
        var isEmailAlreadyRegistered = await _repository.ExistsByEmailAsync(request.Email);
        if (isEmailAlreadyRegistered)
            throw new OnValidationException(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
        var cpf = new Cpf(request.Cpf);
        var isCpfAlreadyRegistered = await _repository.ExistsByCpfAsync(cpf.Value);
        if (isCpfAlreadyRegistered)
            throw new OnValidationException(ResourceErrorMessages.CPF_ALREADY_REGISTERED);
    }
}
