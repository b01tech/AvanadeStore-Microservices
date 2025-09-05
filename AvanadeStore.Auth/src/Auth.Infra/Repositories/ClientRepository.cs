using Auth.Domain.Entities;
using Auth.Domain.Interfaces;
using Auth.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infra.Repositories;
internal class ClientRepository : IClientRepository
{
    private readonly AuthDbContext _context;

    public ClientRepository(AuthDbContext context)
    {
        _context = context;
    }
    public async Task<Client?> GetByIdAsync(Guid id)
    {
        return await _context.Clients.FindAsync(id);
    }
    public async Task<Client?> GetByEmailAndPasswordAsync(string email, string passwordHash)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.IsActive && c.Email.Equals(email) && c.PasswordHash.Equals(passwordHash));
    }
    public async Task<Client?> GetByCpfAndPasswordAsync(string cpf, string passwordHash)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.IsActive && c.Cpf.Value.Equals(c) && c.PasswordHash.Equals(passwordHash));
    }
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Clients.AnyAsync(c => c.Email.Equals(email));
    }
    public async Task<bool> ExistsByCpfAsync(string cpf)
    {
        var clients = await _context.Clients.ToListAsync();
        return clients.Any(c => c.Cpf.Value.Equals(cpf));
    }
    public async Task<Client?> AddAsync(Client client)
    {
        var clientCreated = await _context.Clients.AddAsync(client);
        return await Task.FromResult(clientCreated.Entity);
    }
    public Task<bool> UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        return Task.FromResult(true);
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null) return false;
        else
        {
            client.Deactivate();
            return true;
        }
    }
}
