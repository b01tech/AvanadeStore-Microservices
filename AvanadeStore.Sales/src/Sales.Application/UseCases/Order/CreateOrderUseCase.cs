using Sales.Application.DTOs.Requests;
using Sales.Application.DTOs.Responses;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;
using Sales.Exception.ErrorMessages;

namespace Sales.Application.UseCases.Order;
internal class CreateOrderUseCase : ICreateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseOrderDTO> ExecuteAsync(RequestCreateOrderDTO request)
    {
        if (request.OrderItems == null || !request.OrderItems.Any())
            throw new InvalidArgumentsException(ResourceErrorMessages.ORDER_ITEMS_EMPTY);

        var order = new Domain.Entities.Order();

        foreach (var item in request.OrderItems)
        {
            if (item.Quantity <= 0)
                throw new InvalidArgumentsException(ResourceErrorMessages.QUANTITY_INVALID);

            order.AddOrderItem(item.ProductId, item.Quantity);
        }

        var orderCreated = await _orderRepository.AddAsync(order);
        await _unitOfWork.CommitAsync();

        var orderItems = orderCreated!.OrderItems.Select(oi => new ResponseOrderItemDTO(
            oi.Id,
            oi.ProductId,
            oi.Quantity,
            oi.Price
        )).ToList();

        return new ResponseOrderDTO(
            orderCreated.Id,
            orderCreated.UserId,
            orderCreated.CreatedAt,
            orderCreated.UpdatedAt,
            orderCreated.Total,
            orderCreated.Status,
            orderItems
        );
    }

    public async Task<ResponseOrderDTO> ExecuteAsync(RequestCreateOrderDTO request, Guid userId)
    {
        if (request.OrderItems == null || !request.OrderItems.Any())
            throw new InvalidArgumentsException(ResourceErrorMessages.ORDER_ITEMS_EMPTY);

        var order = new Domain.Entities.Order(userId);

        foreach (var item in request.OrderItems)
        {
            if (item.Quantity <= 0)
                throw new InvalidArgumentsException(ResourceErrorMessages.QUANTITY_INVALID);

            order.AddOrderItem(item.ProductId, item.Quantity);
        }

        var orderCreated = await _orderRepository.AddAsync(order);
        await _unitOfWork.CommitAsync();

        var orderItems = orderCreated!.OrderItems.Select(oi => new ResponseOrderItemDTO(
            oi.Id,
            oi.ProductId,
            oi.Quantity,
            oi.Price
        )).ToList();

        return new ResponseOrderDTO(
            orderCreated.Id,
            orderCreated.UserId,
            orderCreated.CreatedAt,
            orderCreated.UpdatedAt,
            orderCreated.Total,
            orderCreated.Status,
            orderItems
        );
    }
}