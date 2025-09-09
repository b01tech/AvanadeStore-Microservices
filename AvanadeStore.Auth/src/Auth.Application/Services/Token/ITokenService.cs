using Auth.Domain.Entities;

namespace Auth.Application.Services.Token;
public interface ITokenService
{
    string GenereteAccessToken(User user);
}
