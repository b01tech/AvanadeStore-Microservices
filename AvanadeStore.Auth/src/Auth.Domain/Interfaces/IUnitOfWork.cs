namespace Auth.Domain.Interfaces;
public interface IUnitOfWork
{
    Task CommitAsync();
}
