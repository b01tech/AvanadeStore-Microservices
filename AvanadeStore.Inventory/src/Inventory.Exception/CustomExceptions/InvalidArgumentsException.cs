namespace Inventory.Exception.CustomExceptions;
public class InvalidArgumentsException : CustomAppException
{
    public InvalidArgumentsException(string message) : base(message) { }
}
