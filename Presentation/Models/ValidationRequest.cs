using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class ValidationRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

}
