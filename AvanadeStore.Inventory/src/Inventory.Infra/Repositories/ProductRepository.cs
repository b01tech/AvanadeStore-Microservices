using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infra.Repositories;
internal class ProductRepository : IProductRepository
{
    private readonly InventoryDbContext _context;
    private readonly int pageSize = 10;

    public ProductRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = await _context.Products
            .AsNoTracking()            
            .ToListAsync();
        return products;
    }
    public async Task<Product?> GetAsync(long productId)
    {
        return await _context.Products.FindAsync(productId);
    }
    public async Task<bool> GetByName(string productName)
    {
        return await _context.Products.AnyAsync(p => p.Name.ToLowerInvariant() == productName.ToLowerInvariant());
    }
    public async Task<bool> CheckStockAsync(long productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null) return false;
        return product.IsStockAvailable(quantity);
    }
    public async Task<bool> IncreaseStockAsync(long productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null) return false;
        product.IncreseStock(quantity);
        _context.Products.Update(product);
        return true;
    }
    public async Task<bool> DecreaseStockAsync(long productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null) return false;
        product.DecreaseStock(quantity);
        _context.Products.Update(product);
        return true;
    }
    public async Task<Product?> AddAsync(Product product)
    {
        var productCreated = await _context.Products.AddAsync(product);
        return await Task.FromResult(productCreated.Entity);
    }
}
