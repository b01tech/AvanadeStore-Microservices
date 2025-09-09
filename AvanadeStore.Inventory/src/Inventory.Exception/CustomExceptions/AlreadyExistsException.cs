namespace Inventory.Exception.CustomExceptions;
public class AlreadyExistsException : CustomAppException
{
    public AlreadyExistsException(string message) : base(message) { }
}
