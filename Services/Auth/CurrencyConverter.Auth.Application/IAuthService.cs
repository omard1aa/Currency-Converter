using CurrencyConverter.Auth.Application.DTOs;

namespace CurrencyConverter.Auth.Application;
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync(Guid userId);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
}