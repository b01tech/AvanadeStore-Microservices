using Sales.Domain.Interfaces;
using Sales.Infra.Data;

namespace Sales.Infra.Repositories;
internal class UnitOfWork : IUnitOfWork
{
    private readonly SalesDbContext _context;

    public UnitOfWork(SalesDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}