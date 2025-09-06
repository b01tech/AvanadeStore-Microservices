namespace Auth.Application.DTOs.Requests;
public record RequestLoginByCpfDTO(string cpf, string password)
{
    public RequestLoginByCpfDTO WithHashedPassword(string hashedPaswword) => this with { password = hashedPaswword };
}

public record RequestLoginByEmailDTO(string email, string password)
{
    public RequestLoginByEmailDTO WithHashedPassword(string hashedPaswword) => this with { password = hashedPaswword };

}
