using Sales.Application.DTOs.Responses;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Interfaces;

namespace Sales.Application.UseCases.Order;
public class UpdateOrderStatusUseCase : IUpdateOrderStatusUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseOrderDTO> ExecuteConfirmSeparationAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException("Order not found");

        if (order.Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Order must be in Confirmed status to start separation");

        order.StartSeparation();
        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.CommitAsync();

        return new ResponseOrderDTO(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.UpdatedAt,
            order.Total,
            order.Status,
            order.OrderItems.Select(oi => new ResponseOrderItemDTO(
                oi.Id,
                oi.ProductId,
                oi.Quantity,
                oi.Price
            )).ToList()
        );
    }

    public async Task<ResponseOrderDTO> ExecuteCancelOrderAsync(Guid orderId, Guid userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException("Order not found");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException("User can only cancel their own orders");

        if (order.Status != OrderStatus.Confirmed && order.Status != OrderStatus.InSeparation)
            throw new InvalidOperationException("Order must be in Confirmed or InSeparation status to be cancelled");

        order.CancelOrder();
        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.CommitAsync();

        return new ResponseOrderDTO(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.UpdatedAt,
            order.Total,
            order.Status,
            order.OrderItems.Select(oi => new ResponseOrderItemDTO(
                oi.Id,
                oi.ProductId,
                oi.Quantity,
                oi.Price
            )).ToList()
        );
    }
}