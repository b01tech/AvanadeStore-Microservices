using Auth.Domain.Enums;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;
public class Client : User
{
    public Cpf Cpf { get; init; }

    public Client(string name, string email, string passwordHash, Cpf cpf)
        : base(name, email, passwordHash, UserRole.Client)
    {
        Cpf = cpf;
    }
}
