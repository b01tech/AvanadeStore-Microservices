namespace Sales.Application.DTOs.Messages;

public record OrderValidatedMessage(
    Guid OrderId,
    List<OrderValidatedItem> Items,
    bool IsValid
);

public record OrderValidatedItem(
    long ProductId,
    int Quantity,
    decimal Price,
    bool HasStock
);
