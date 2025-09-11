using Sales.Application.DTOs.Requests;
using Sales.Application.DTOs.Responses;
using Sales.Application.DTOs.Messages;
using Sales.Application.Services.MessageBus;
using Sales.Domain.Interfaces;
using Sales.Exception.CustomExceptions;
using Sales.Exception.ErrorMessages;

namespace Sales.Application.UseCases.Order;
internal class CreateOrderUseCase : ICreateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _bus;


    public CreateOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IMessageBus bus)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _bus = bus;
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
        
        await PublishStockValidationMessage(orderCreated!);

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

        await PublishStockValidationMessage(orderCreated!);

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

    private async Task PublishStockValidationMessage(Domain.Entities.Order order)
    {
        var stockValidationItems = order.OrderItems.Select(oi => new StockValidationItem(
            oi.ProductId,
            oi.Quantity
        )).ToList();

        var message = new StockValidationMessage(
            order.Id,
            stockValidationItems
        );

        await _bus.PublishAsync(QueueNames.STOCK_VALIDATION_QUEUE, message);
    }
}
