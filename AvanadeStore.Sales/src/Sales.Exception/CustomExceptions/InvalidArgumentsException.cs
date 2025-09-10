namespace Sales.Exception.CustomExceptions;

public class InvalidArgumentsException : CustomAppException
{
    public InvalidArgumentsException(string message) : base(message)
    {
    }

    public InvalidArgumentsException(IList<string> errorMessages) : base(errorMessages)
    {
    }
}