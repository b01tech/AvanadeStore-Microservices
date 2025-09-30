namespace Auth.Domain.Interfaces;
public interface IClientRepository
{
    Task<Entities.Client?> GetByIdAsync(Guid id);
    Task<Entities.Client?> GetByEmailAndPasswordAsync(string email, string passwordHash);
    Task<Entities.Client?> GetByCpfAndPasswordAsync(string cpf, string passwordHash);
    Task<bool> ExistsByCpfAsync(string cpf);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Entities.Client?> AddAsync(Entities.Client client);
    Task<bool> UpdateAsync(Entities.Client client);
    Task<bool> SoftDeleteAsync(Guid id);
}
