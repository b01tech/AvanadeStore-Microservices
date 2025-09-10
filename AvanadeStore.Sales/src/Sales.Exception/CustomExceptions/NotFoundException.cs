namespace Sales.Exception.CustomExceptions;

public class NotFoundException : CustomAppException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(IList<string> errorMessages) : base(errorMessages)
    {
    }
}