using Sales.Domain.Entities;

namespace Sales.Domain.Interfaces;
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task<Order?> AddAsync(Order order);
    Task<bool> UpdateAsync(Order order);
    Task<bool> DeleteAsync(Guid id);
}