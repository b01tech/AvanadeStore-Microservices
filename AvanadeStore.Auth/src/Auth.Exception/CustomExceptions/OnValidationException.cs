namespace Auth.Exception.CustomExceptions;
public class OnValidationException : CustomAppException
{
    public OnValidationException(string errorMessage) : base(errorMessage) { }

}
