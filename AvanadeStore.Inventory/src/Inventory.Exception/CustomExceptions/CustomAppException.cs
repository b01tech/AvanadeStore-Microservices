namespace Inventory.Exception.CustomExceptions;
public abstract class CustomAppException : System.Exception
{
    public IList<string> ErrorMessages { get; set; }

    protected CustomAppException(string message) : base(message)
    {
        ErrorMessages = new List<string> { message };
    }

    protected CustomAppException(IList<string> errorMessages)
    {
        ErrorMessages = errorMessages;
    }
}
