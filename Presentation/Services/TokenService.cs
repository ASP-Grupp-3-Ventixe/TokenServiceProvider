using Microsoft.IdentityModel.Tokens;
using Presentation.Interfaces;
using Presentation.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Presentation.Services;

public class TokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secretKey;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenService()
    {
        _issuer = GetEnv("Issuer");
        _audience = GetEnv("Audience");
        _secretKey = GetEnv("SecretKey");
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<TokenResponse> GenerateTokenAsync(TokenRequest request, int expiresInDays = 30)
    {

        try
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                return TokenResponse.Fail("UserId is required");

            var claims = BuildClaims(request);
            var credentials = GetSigningCredentials();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddDays(expiresInDays)
            };

            var token = _tokenHandler.CreateToken(descriptor);


            return TokenResponse.Success(_tokenHandler.WriteToken(token));

        }
        catch (Exception ex)
        {
            return TokenResponse.Fail($"GenerateTokenAsync failed... {ex}");
        }
    }

    public async Task<ValidationResponse> ValidateTokenAsync(ValidationRequest request)
    {

        try
        {
            var parameters = GetValidationParameters();
            var principal = _tokenHandler.ValidateToken(request.AccessToken, parameters, out _);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || userId != request.UserId)
                return ValidationResponse.Fail("UserId does not match token.");

            return ValidationResponse.Success("Access granted");
        }
        catch (Exception ex)
        {
            return ValidationResponse.Fail($"ValidateTokenAsync failed... {ex}");
        }
    }

    #region Helpers
    private static string GetEnv(string key) =>
        Environment.GetEnvironmentVariable(key) ?? throw new ArgumentNullException(key, $"Environment variable '{key}' is missing.");

    private SigningCredentials GetSigningCredentials() =>
        new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            SecurityAlgorithms.HmacSha256);

    private static List<Claim> BuildClaims(TokenRequest request)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, request.UserId)
        };

        if (!string.IsNullOrEmpty(request.Email))
            claims.Add(new Claim(ClaimTypes.Email, request.Email));
        if (!string.IsNullOrEmpty(request.Role))
            claims.Add(new Claim(ClaimTypes.Role, request.Role));

        return claims;
    }

    private TokenValidationParameters GetValidationParameters() =>
        new()
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ClockSkew = TimeSpan.Zero,
        };
    #endregion
}
