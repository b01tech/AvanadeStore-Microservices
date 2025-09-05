namespace Auth.Exception.CustomExceptions;
public class LoadEnvSettingsException : CustomAppException
{
    public LoadEnvSettingsException(string errorMessage) : base(errorMessage) { }
    public LoadEnvSettingsException(List<string> errorMessages) : base(errorMessages) { }
}
