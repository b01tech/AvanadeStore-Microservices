namespace Sales.Domain.Interfaces;
public interface IUnitOfWork
{
    Task CommitAsync();
}