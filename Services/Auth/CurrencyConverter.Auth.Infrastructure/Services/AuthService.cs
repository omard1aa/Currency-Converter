using CurrencyConverter.Auth.Application;
using CurrencyConverter.Auth.Application.DTOs;
using CurrencyConverter.Auth.Infrastructure.Persistence;
using CurrencyConverter.Auth.Application.Services;
using CurrencyConverter.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConverter.Auth.Infrastructure.Services;
public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    public AuthService(AuthDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Username);
        
        if (existingUser != null)
            throw new InvalidOperationException("User with this email or username already exists");
        
        // Hash Password
        var hashedPassword = HashPassword(request.Password);

        // Create new user
        var newUser = User.Create(request.Email, request.Username, hashedPassword, request.FirstName, request.LastName);
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // Assign default role
        var defaultRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == Role.Roles.User)
            ?? throw new InvalidOperationException("Default role not found");
        _context.UserRoles.Add(new UserRole { UserId = newUser.Id, RoleId = defaultRole.Id });

        await _context.SaveChangesAsync();

        // Generate tokens
        var roles = new[] { Role.Roles.User };
        var accessToken = _jwtService.GenerateAccessToken(newUser, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = RefreshToken.Create(newUser.Id, refreshToken, DateTime.UtcNow.AddDays(7), "");
        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();
        return new AuthResponse
        {
            UserId = newUser.Id,
            Username = newUser.Username,
            Email = newUser.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Roles = roles
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Implementation for user login
        var user = await _context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Email == request.Email);
        if(user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        user.UpdateLastLogin();
        await _context.SaveChangesAsync();

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = RefreshToken.Create(user.Id, refreshToken, DateTime.UtcNow.AddDays(7), "");
        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Roles = roles
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
            throw new InvalidOperationException("Invalid or expired refresh token");

        var user = token.User;
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        token.Revoke("", newRefreshToken);
        var newRefreshTokenEntity = RefreshToken.Create(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7), "");
        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Roles = roles
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        // Query actual database columns instead of computed properties
        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in refreshTokens)
        {
            token.Revoke("");
        }

        await _context.SaveChangesAsync();
    }

    public static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string hashedPassword) =>
        HashPassword(password) == hashedPassword;
}