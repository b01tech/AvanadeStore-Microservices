using Auth.Domain.Enums;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Domain.Entities;
public abstract class User
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    protected User(string name, string email, string passwordHash, UserRole role)
    {
        Validate(name, email, passwordHash);
        ValidateRole(role);
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void Update(string name, string email, string passwordHash)
    {
        Validate(name, email, passwordHash);
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new OnValidationException(ResourceErrorMessages.NAME_EMPTY);
        if (string.IsNullOrWhiteSpace(email))
            throw new OnValidationException(ResourceErrorMessages.EMAIL_EMPTY);
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new OnValidationException(ResourceErrorMessages.PASSWORD_EMPTY);
    }
    private static void ValidateRole(UserRole role)
    {
        if (!Enum.IsDefined<UserRole>(role))
            throw new OnValidationException(ResourceErrorMessages.USER_ROLE_INVALID);
    }

}
