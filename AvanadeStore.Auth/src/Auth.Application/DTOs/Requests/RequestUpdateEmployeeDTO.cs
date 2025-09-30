using Auth.Domain.Enums;

namespace Auth.Application.DTOs.Requests;

public record RequestUpdateEmployeeDTO(string Name, string Email, UserRole Role);