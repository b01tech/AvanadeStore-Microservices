using Sales.Application.DTOs.Responses;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;
using Sales.Exception.ErrorMessages;

namespace Sales.Application.UseCases.Order;
internal class GetOrderUseCase : IGetOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<ResponseOrderDTO> ExecuteAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id) ?? throw new OrderNotFoundException(id);
        
        var orderItems = order.OrderItems.Select(oi => new ResponseOrderItemDTO(
            oi.Id,
            oi.ProductId,
            oi.Quantity,
            oi.Price
        )).ToList();

        return new ResponseOrderDTO(
            order.Id,
            order.CreatedAt,
            order.UpdatedAt,
            order.Total,
            order.Status,
            orderItems
        );
    }

    public async Task<ResponseOrdersListDTO> ExecuteGetAllAsync(int page = 1)
    {
        const int pageSize = 10;
        var orders = await _orderRepository.GetAllAsync();
        var totalItems = orders.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
        var ordersList = orders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new ResponseOrderDTO(
                o.Id,
                o.CreatedAt,
                o.UpdatedAt,
                o.Total,
                o.Status,
                o.OrderItems.Select(oi => new ResponseOrderItemDTO(
                    oi.Id,
                    oi.ProductId,
                    oi.Quantity,
                    oi.Price
                )).ToList()
            ))
            .ToList();
            
        return new ResponseOrdersListDTO(ordersList, page, totalItems, totalPages);
    }

    public async Task<ResponseOrdersListDTO> ExecuteGetByUserIdAsync(Guid userId, int page = 1)
    {
        const int pageSize = 10;
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var totalItems = orders.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
        var ordersList = orders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new ResponseOrderDTO(
                o.Id,
                o.CreatedAt,
                o.UpdatedAt,
                o.Total,
                o.Status,
                o.OrderItems.Select(oi => new ResponseOrderItemDTO(
                    oi.Id,
                    oi.ProductId,
                    oi.Quantity,
                    oi.Price
                )).ToList()
            ))
            .ToList();
            
        return new ResponseOrdersListDTO(ordersList, page, totalItems, totalPages);
    }
}