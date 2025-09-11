namespace Inventory.Exception.CustomExceptions;
public class MessageFailException : CustomAppException
{
    public MessageFailException(string message) : base(message)
    {
    }
}
