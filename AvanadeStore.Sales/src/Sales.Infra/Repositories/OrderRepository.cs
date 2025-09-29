using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using Sales.Domain.Enums;
using Sales.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Sales.Infra.Repositories;
internal class OrderRepository : IOrderRepository
{
    private readonly SalesDbContext _context;

    public OrderRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Status == status)
            .ToListAsync();
    }

    public async Task<Order?> AddAsync(Order order)
    {
        var orderCreated = await _context.Orders.AddAsync(order);
        return await Task.FromResult(orderCreated.Entity);
    }

    public async Task<bool> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null)
            return false;
        
        _context.Orders.Remove(order);
        return true;
    }
}