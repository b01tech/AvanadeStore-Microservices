using Sales.Exception.ErrorMessages;

namespace Sales.Exception.CustomExceptions;

public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(Guid orderId) 
        : base($"Pedido com ID {orderId} não foi encontrado")
    {
    }

    public OrderNotFoundException(string message) : base(message)
    {
    }

    public OrderNotFoundException(IList<string> errorMessages) : base(errorMessages)
    {
    }
}