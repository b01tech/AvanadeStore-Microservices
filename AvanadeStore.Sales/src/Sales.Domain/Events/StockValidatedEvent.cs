namespace Sales.Domain.Events;
public record StockValidatedEvent(Guid OrderId, bool IsAvailable);
