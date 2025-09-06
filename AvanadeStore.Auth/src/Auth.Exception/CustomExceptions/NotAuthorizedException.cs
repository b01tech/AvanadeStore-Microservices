namespace Auth.Exception.CustomExceptions;
public class NotAuthorizedException : CustomAppException
{
    public NotAuthorizedException(string errorMessage)
        : base(errorMessage)
    { }
}
