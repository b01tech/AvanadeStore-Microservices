namespace Sales.Application.DTOs.Messages;

public record OrderFinishedMessage(
    Guid OrderId,
    List<OrderFinishedItem> Items
);

public record OrderFinishedItem(
    long ProductId,
    int Quantity
);