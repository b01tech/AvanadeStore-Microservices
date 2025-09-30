namespace Auth.Exception.CustomExceptions;

public class UnauthorizedException : CustomAppException
{
    public UnauthorizedException(string errorMessage)
        : base(errorMessage)
    { }
}