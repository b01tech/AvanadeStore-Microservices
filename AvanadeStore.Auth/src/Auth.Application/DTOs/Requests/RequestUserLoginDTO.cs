namespace Auth.Application.DTOs.Requests;
public record RequestLoginByCpfDTO(string Cpf, string Password);

public record RequestLoginByEmailDTO(string Email, string Password);
