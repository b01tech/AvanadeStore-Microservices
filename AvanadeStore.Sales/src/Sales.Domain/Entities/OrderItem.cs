using Sales.Exception.CustomExceptions;
using Sales.Exception.ErrorMessages;

namespace Sales.Domain.Entities;
public class OrderItem
{
    public Guid Id { get; init; }
    public long ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid OrderId { get; private set; }

    public OrderItem(long productId, int quantity, decimal price)
    {
        Validate(productId, quantity, price);
        Id = Guid.CreateVersion7();
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    protected OrderItem() { }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new OnValidationException(ResourceErrorMessages.QUANTITY_INVALID);
        Quantity = quantity;
    }

    public void UpdatePrice(decimal price)
    {
        if (price <= 0)
            throw new OnValidationException(ResourceErrorMessages.PRICE_INVALID);
        Price = price;
    }

    public decimal GetSubTotal() => Quantity * Price;

    private static void Validate(long productId, int quantity, decimal price)
    {
        if (productId <= 0)
            throw new OnValidationException(ResourceErrorMessages.PRODUCT_ID_INVALID);
        if (quantity <= 0)
            throw new OnValidationException(ResourceErrorMessages.QUANTITY_INVALID);
        if (price <= 0)
            throw new OnValidationException(ResourceErrorMessages.PRICE_INVALID);
    }

    internal void SetOrderId(Guid orderId)
    {
        OrderId = orderId;
    }
}