using Sales.Domain.Enums;
using Sales.Exception.CustomExceptions;
using Sales.Exception.ErrorMessages;

namespace Sales.Domain.Entities;
public class Order
{
    public Guid Id { get; init; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public Order()
    {
        Id = Guid.CreateVersion7();
        UserId = Guid.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = OrderStatus.Created;
        Total = 0;
    }

    public Order(Guid userId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = OrderStatus.Created;
        Total = 0;
    }

    public void AddOrderItem(long productId, int quantity, decimal price)
    {
        ValidateCanModifyItems();

        var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = new OrderItem(productId, quantity, price);
            orderItem.SetOrderId(Id);
            _orderItems.Add(orderItem);
        }

        CalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddOrderItem(long productId, int quantity)
    {
        ValidateCanModifyItems();

        var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = new OrderItem(productId, quantity, 1m);
            orderItem.SetOrderId(Id);
            _orderItems.Add(orderItem);
        }

        CalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveOrderItem(long productId)
    {
        ValidateCanModifyItems();

        var item = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            _orderItems.Remove(item);
            CalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateOrderItemQuantity(long productId, int quantity)
    {
        ValidateCanModifyItems();

        var item = _orderItems.FirstOrDefault(x => x.ProductId == productId)
            ?? throw new NotFoundException(ResourceErrorMessages.ORDER_ITEM_NOT_FOUND);

        item.UpdateQuantity(quantity);
        CalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrderItemPrice(long productId, decimal price)
    {
        ValidateCanModifyItems();

        var item = _orderItems.FirstOrDefault(x => x.ProductId == productId)
            ?? throw new NotFoundException(ResourceErrorMessages.ORDER_ITEM_NOT_FOUND);

        item.UpdatePrice(price);
        CalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmOrder()
    {
        ValidateStatusTransition(OrderStatus.Confirmed);
        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RejectOrder()
    {
        ValidateStatusTransition(OrderStatus.Rejected);
        Status = OrderStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartSeparation()
    {
        ValidateStatusTransition(OrderStatus.InSeparation);
        Status = OrderStatus.InSeparation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelOrder()
    {
        ValidateCanCancel();
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void FinishOrder()
    {
        ValidateStatusTransition(OrderStatus.Finished);
        Status = OrderStatus.Finished;
        UpdatedAt = DateTime.UtcNow;
    }

    private void CalculateTotal()
    {
        Total = _orderItems.Sum(item => item.GetSubTotal());
    }

    private void ValidateCanModifyItems()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOrderStatusException(ResourceErrorMessages.ORDER_CANNOT_MODIFY_ITEMS);
    }

    private void ValidateStatusTransition(OrderStatus newStatus)
    {
        var validTransitions = GetValidTransitions();
        if (!validTransitions.Contains(newStatus))
            throw new OnValidationException(ResourceErrorMessages.ORDER_INVALID_STATUS_TRANSITION);
    }

    private void ValidateCanCancel()
    {
        if (Status != OrderStatus.Confirmed && Status != OrderStatus.InSeparation)
            throw new OnValidationException(ResourceErrorMessages.ORDER_CANNOT_CANCEL);
    }

    private List<OrderStatus> GetValidTransitions()
    {
        return Status switch
        {
            OrderStatus.Created => [OrderStatus.Confirmed, OrderStatus.Rejected],
            OrderStatus.Confirmed => [OrderStatus.InSeparation, OrderStatus.Cancelled],
            OrderStatus.InSeparation => [OrderStatus.Finished, OrderStatus.Cancelled],
            _ => []
        };
    }
}
