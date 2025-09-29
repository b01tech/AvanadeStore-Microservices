namespace Inventory.Application.Services.MessageBus;

public static class QueueNames
{
    public const string STOCK_VALIDATION_QUEUE = "stock-validation-queue";
    public const string ORDER_VALIDATED_QUEUE = "order-validated";
    public const string ORDER_FINISHED_QUEUE = "order-finished";
}