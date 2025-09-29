using Sales.Application.DTOs.Responses;
using Sales.Domain.Interfaces;
using Sales.Domain.Enums;
using Sales.Exception.CustomExceptions;
using Sales.Exception.ErrorMessages;

namespace Sales.Application.UseCases.Order;
internal class UpdateOrderStatusUseCase : IUpdateOrderStatusUseCase
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
        var order = await _orderRepository.GetByIdAsync(orderId) 
            ?? throw new OrderNotFoundException(orderId);

        // Regra de negócio: a order deve estar Confirmed para ir para InSeparation
        if (order.Status != OrderStatus.Confirmed)
        {
            throw new OnValidationException("Order must be in Confirmed status to move to InSeparation");
        }

        order.StartSeparation();
        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.CommitAsync();

        var orderItems = order.OrderItems.Select(oi => new ResponseOrderItemDTO(
            oi.Id,
            oi.ProductId,
            oi.Quantity,
            oi.Price
        )).ToList();

        return new ResponseOrderDTO(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.UpdatedAt,
            order.Total,
            order.Status,
            orderItems
        );
    }
}