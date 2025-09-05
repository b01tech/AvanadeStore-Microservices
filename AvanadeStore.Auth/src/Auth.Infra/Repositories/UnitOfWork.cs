using Auth.Domain.Interfaces;
using Auth.Infra.Data;

namespace Auth.Infra.Repositories;
internal class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;

    public UnitOfWork(AuthDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}
