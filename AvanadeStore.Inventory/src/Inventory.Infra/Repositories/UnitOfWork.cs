using Inventory.Domain.Interfaces;
using Inventory.Infra.Data;

namespace Inventory.Infra.Repositories;
internal class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;

    public UnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}
