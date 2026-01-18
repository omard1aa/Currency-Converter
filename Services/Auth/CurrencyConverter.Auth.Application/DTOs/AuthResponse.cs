namespace CurrencyConverter.Auth.Application.DTOs;
public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public IEnumerable<string> Roles { get; set; }
}