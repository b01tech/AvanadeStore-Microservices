using Auth.Domain.Enums;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Domain.Entities;
public class Employee : User
{
    public Employee(string name, string email, string passwordHash, UserRole role)
        : base(name, email, passwordHash, role)
    {
        ValidateRole(role);
    }

    private static void ValidateRole(UserRole role)
    {
        if (role == UserRole.Client)
            throw new OnValidationException(ResourceErrorMessages.EMPLOYEE_ROLE_INVALID);
    }
}
