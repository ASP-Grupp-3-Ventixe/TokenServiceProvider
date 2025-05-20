using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class TokenResponse
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public string? Message { get; set; }

    public static TokenResponse Success(string token) =>
        new() { Succeeded = true, AccessToken = token, Message = "Token generated." };

    public static TokenResponse Fail(string message) =>
        new() { Succeeded = false, Message = message };

}
