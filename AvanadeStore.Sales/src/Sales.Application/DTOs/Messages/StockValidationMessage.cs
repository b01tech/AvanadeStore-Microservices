namespace Sales.Application.DTOs.Messages;

public record StockValidationMessage(
    Guid OrderId,
    List<StockValidationItem> Items
);

public record StockValidationItem(
    long ProductId,
    int Quantity
);