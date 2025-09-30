using Sales.Application.DTOs.Messages;
using Sales.Application.DTOs.Responses;
using Sales.Application.Services.MessageBus;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;

namespace Sales.Application.UseCases.Order;
public class UpdateOrderStatusUseCase : IUpdateOrderStatusUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;

    public UpdateOrderStatusUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IMessageBus messageBus)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
    }

    public async Task<ResponseOrderDTO> ExecuteConfirmSeparationAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException("Order not found");

        if (order.Status != OrderStatus.Confirmed)
            throw new InvalidOrderStatusException("Pedido deve estar no status Confirmado para iniciar separação");

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
            throw new InvalidOrderStatusException("Pedido só pode ser cancelado se estiver Confirmado ou Em Separação");

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

    public async Task<ResponseOrderDTO> ExecuteFinishOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException("Order not found");

        if (order.Status != OrderStatus.InSeparation)
            throw new InvalidOrderStatusException("Pedido deve estar Em Separação para ser finalizado");

        order.FinishOrder();
        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.CommitAsync();

        // Emitir mensagem para diminuir estoque
        var orderFinishedMessage = new OrderFinishedMessage(
            order.Id,
            order.OrderItems.Select(oi => new OrderFinishedItem(
                oi.ProductId,
                oi.Quantity
            )).ToList()
        );

        await _messageBus.PublishAsync(QueueNames.ORDER_FINISHED_QUEUE, orderFinishedMessage);

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