using System.Security.Claims;
using CurrencyConverter.Auth.Domain.Entities;

namespace CurrencyConverter.Auth.Application.Services;
public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal ValidateToken(string token);
}