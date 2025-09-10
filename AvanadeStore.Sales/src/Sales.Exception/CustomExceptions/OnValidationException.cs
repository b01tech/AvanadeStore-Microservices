namespace Sales.Exception.CustomExceptions;

public class OnValidationException : CustomAppException
{
    public OnValidationException(string message) : base(message)
    {
    }

    public OnValidationException(IList<string> errorMessages) : base(errorMessages)
    {
    }
}