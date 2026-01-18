using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CurrencyConverter.Auth.Application;
using CurrencyConverter.Auth.Application.Services;
using CurrencyConverter.Auth.Infrastructure.Services;
using CurrencyConverter.Auth.Infrastructure.Persistence;
using CurrencyConverter.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Authentication with JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and seed default roles on startup
async Task SeedDatabase()
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            
            // Apply any pending migrations
            await context.Database.MigrateAsync();
            
            // Seed default roles if they don't exist
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    Role.Create(Role.Roles.Admin, "Administrator role with full access"),
                    Role.Create(Role.Roles.User, "Regular user role with limited access")
                );
                await context.SaveChangesAsync();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning($"Database migration failed (PostgreSQL might not be running): {ex.Message}");
    }
}

await SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
