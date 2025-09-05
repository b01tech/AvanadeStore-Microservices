using Auth.Domain.Enums;

namespace Auth.Application.DTOs.Requests;
public record RequestCreateClientDTO(string Name, string Email, string Cpf, string Password);
public record RequestCreateEmployeeDTO(string Name, string Email, string Password, UserRole Role);
