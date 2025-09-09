namespace Inventory.Domain.Interfaces;
public interface IUnitOfWork
{
    Task CommitAsync();
}
