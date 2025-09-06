namespace Auth.Application.DTOs.Responses;
public record ResponseUserToken(string Username, DateTime ExpirationAt, string Token);
