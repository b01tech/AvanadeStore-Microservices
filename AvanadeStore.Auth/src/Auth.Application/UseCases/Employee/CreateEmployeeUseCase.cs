using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Services.Criptography;
using Auth.Domain.Interfaces;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;
using MapsterMapper;

namespace Auth.Application.UseCases.Employee;
internal class CreateEmployeeUseCase : ICreateEmployeeUseCase
{
    private readonly IEncrypter _encrypter;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeRepository _repository;

    public CreateEmployeeUseCase(IEncrypter encrypter, IMapper mapper, IUnitOfWork unitOfWork, IEmployeeRepository repository)
    {
        _encrypter = encrypter;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<ResponseCreateUserDTO> ExecuteAsync(RequestCreateEmployeeDTO request)
    {
        await ValidateAsync(request);
        var hashedPassword = _encrypter.Encrypt(request.Password);
        var employee = _mapper.Map<Domain.Entities.Employee>(request.WithHashedPassword(hashedPassword));

        await _repository.AddAsync(employee);
        await _unitOfWork.CommitAsync();

        var response = _mapper.Map<ResponseCreateUserDTO>(employee);
        return await Task.FromResult(response);
    }
    private async Task ValidateAsync(RequestCreateEmployeeDTO request)
    {
        var validator = new CreateEmployeeValidator();
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new OnValidationException(errorMessages);
        }
        var isEmailAlreadyRegistered = await _repository.ExistsByEmailAsync(request.Email);
        if (isEmailAlreadyRegistered)
            throw new OnValidationException(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
    }
}
