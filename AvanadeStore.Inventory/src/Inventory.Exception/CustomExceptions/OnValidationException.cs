namespace Inventory.Exception.CustomExceptions;
public class OnValidationException : CustomAppException
{
    public OnValidationException(string errorMessage) : base(errorMessage) { }
    public OnValidationException(List<string> errorMessages) : base(errorMessages) { }
}
