namespace Sales.Application.DTOs.Responses;
public class ResponseErrorMessage
{
    public int StatusCode { get; protected set; }
    public List<string> Messages { get; protected set; } = new List<string>();
    public ResponseErrorMessage(int statusCode, List<string> messages)
    {
        StatusCode = statusCode;
        Messages = messages;
    }
    public ResponseErrorMessage(int statusCode, string message)
    {
        StatusCode = statusCode;
        Messages.Add(message);
    }
}
