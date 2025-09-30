using Sales.Exception.ErrorMessages;

namespace Sales.Exception.CustomExceptions;

public class InvalidOrderStatusException : CustomAppException
{
    public InvalidOrderStatusException() : base(ResourceErrorMessages.INVALID_ORDER_STATUS_OPERATION)
    {
    }

    public InvalidOrderStatusException(string message) : base(message)
    {
    }

    public InvalidOrderStatusException(List<string> errorMessages) : base(errorMessages)
    {
    }
}