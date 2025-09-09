using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(int page = 1);
    Task<Product?> GetAsync(long productId);
    Task<bool> CheckStockAsync(long productId, int quantity);
    Task<bool> DecreaseStockAsync(long productId, int quantity);
    Task<bool> IncreaseStockAsync(long productId, int quantity);
    Task<Product?> AddAsync(Product product);
}
