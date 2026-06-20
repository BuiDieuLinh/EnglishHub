namespace EnglishHub.Application.DTOs;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Role { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}
