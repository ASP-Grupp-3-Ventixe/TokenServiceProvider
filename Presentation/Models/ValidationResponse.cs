namespace Presentation.Models;
public class ValidationResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }

    public static ValidationResponse Success(string message) =>
        new() { Succeeded = true, Message = message };

    public static ValidationResponse Fail(string message) =>
        new() { Succeeded = false, Message = message };
}

