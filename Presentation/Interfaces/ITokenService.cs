

using Presentation.Models;

namespace Presentation.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(TokenRequest request, int expiresInDays = 30);
        Task<ValidationResponse> ValidateAccessTokenAsync(ValidationRequest request);
    }
}