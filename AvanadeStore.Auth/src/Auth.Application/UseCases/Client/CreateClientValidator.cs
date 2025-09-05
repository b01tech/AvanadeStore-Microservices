using Auth.Application.DTOs.Requests;
using Auth.Exception.ErrorMessages;
using FluentValidation;

namespace Auth.Application.UseCases.Client;
public class CreateClientValidator : AbstractValidator<RequestCreateClientDTO>
{
    public CreateClientValidator()
    {
        RuleFor(u => u.Name).NotEmpty().WithMessage(ResourceErrorMessages.NAME_EMPTY);
        RuleFor(u => u.Email).NotEmpty().WithMessage(ResourceErrorMessages.EMAIL_EMPTY);
        RuleFor(u => u.Password.Length).GreaterThanOrEqualTo(6).WithMessage(ResourceErrorMessages.PASSWORD_INVALID);
        When(u => !string.IsNullOrEmpty(u.Email), () =>
        {
            RuleFor(u => u.Email).EmailAddress().WithMessage(ResourceErrorMessages.EMAIL_INVALID);
        });
    }
}
