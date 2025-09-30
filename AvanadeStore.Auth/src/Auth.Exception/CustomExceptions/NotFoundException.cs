namespace Auth.Exception.CustomExceptions;

public class NotFoundException : CustomAppException
{
    public NotFoundException(string errorMessage)
        : base(errorMessage)
    { }
}