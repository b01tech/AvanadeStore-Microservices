using Auth.Application.DTOs.Requests;
using Auth.Domain.Enums;
using Auth.Exception.ErrorMessages;
using FluentValidation;

namespace Auth.Application.UseCases.Employee;
internal class CreateEmployeeValidator : AbstractValidator<RequestCreateEmployeeDTO>
{
    public CreateEmployeeValidator()
    {
        RuleFor(u => u.Name).NotEmpty().WithMessage(ResourceErrorMessages.NAME_EMPTY);
        RuleFor(u => u.Email).NotEmpty().WithMessage(ResourceErrorMessages.EMAIL_EMPTY);
        RuleFor(u => u.Password.Length).GreaterThanOrEqualTo(6).WithMessage(ResourceErrorMessages.PASSWORD_INVALID);
        When(u => !string.IsNullOrEmpty(u.Email), () =>
        {
            RuleFor(u => u.Email).EmailAddress().WithMessage(ResourceErrorMessages.EMAIL_INVALID);
        });
        RuleFor(u => u.Role).IsInEnum().WithMessage(ResourceErrorMessages.ROLE_INVALID)
            .NotEqual(UserRole.Client).WithMessage(ResourceErrorMessages.ROLE_CLIENT_NOT_ALLOWED);
    }
}
