namespace Auth.Domain.Interfaces;
public interface IEmployeeRepository
{
    Task<Entities.Employee?> GetByIdAsync(Guid id);
    Task<Entities.Employee?> GetByEmailAndPasswordAsync(string email, string passwordHash);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Entities.Employee?> AddAsync(Entities.Employee employee);
    Task<bool> UpdateAsync(Entities.Employee employee);
    Task<bool> SoftDeleteAsync(Guid id);
}
