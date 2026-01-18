// CurrencyConverter.Auth.Infrastructure/Services/JwtService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CurrencyConverter.Auth.Domain.Entities;

namespace CurrencyConverter.Auth.Application.Services;
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = configuration["Jwt:SecretKey"] 
            ?? throw new InvalidOperationException("JWT Secret Key not configured");
        _issuer = configuration["Jwt:Issuer"] ?? "CurrencyConverterAPI";
        _audience = configuration["Jwt:Audience"] ?? "CurrencyConverterAPI";
        _accessTokenExpirationMinutes = int.Parse(configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
    }
    
    public string GenerateAccessToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", user.Username),
            new Claim("clientId", user.Id.ToString()) // For logging requirement
        };
        
        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No tolerance for expiration
            }, out SecurityToken validatedToken);
            
            return principal;
        }
        catch (Exception ex)
        {
            // Token validation failed
            throw new SecurityTokenException("Invalid token", ex);
        }
    }
}
