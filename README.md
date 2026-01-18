# Currency Converter API

ASP.NET Core REST API with JWT authentication and currency exchange functionality.

## 📋 Overview

A secure, scalable Currency Converter API built with Clean Architecture principles, featuring:
- JWT-based authentication with refresh token support
- PostgreSQL database with Entity Framework Core
- Role-based access control (Admin/User roles)
- Clean Architecture with 4-layer separation
- Comprehensive error handling and validation

## 🚀 Getting Started

### Prerequisites
- **.NET 8.0 SDK** 
- **PostgreSQL 15+**
- **zsh or bash** shell

### 1. Database Setup

Ensure PostgreSQL is running locally. The application will auto-migrate on startup.

```zsh
# Optional: Create database manually (app creates it automatically)
createdb CurrencyConverterAuthDb
```

### 2. Configure Connection String

Update `/Currency-Converter/Services/Auth/CurrencyConverter.Auth.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CurrencyConverterAuthDb;Username=postgres;Password=postgres;"
  },
  "Jwt": {
    "SecretKey": "ThisIsAVerySecureSecretKeyWithMoreThan32Characters!@#$%",
    "Issuer": "CurrencyConverterAPI",
    "Audience": "CurrencyConverterAPI",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 3. Start the Server

```zsh
# Navigate to the API project
cd "Currency-Converter/Services/Auth/CurrencyConverter.Auth.API"

# Build the solution
dotnet build

# Run the server
dotnet run
```

The API will start on **http://localhost:5204**

### 4. Access Swagger UI

Open your browser: **http://localhost:5204/swagger**

## 🔐 Authentication Endpoints

### Register User
```bash
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "username": "john_doe",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePass123!"
}
```

**Response:**
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "username": "john_doe",
  "email": "user@example.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "base64encodedtoken...",
  "roles": ["User"]
}
```

### Login
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

### Logout (Protected)
```bash
POST /api/auth/logout
Authorization: Bearer {accessToken}
```

Revokes all refresh tokens for the user.

### Refresh Token
```bash
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "base64encodedtoken..."
}
```

Returns new access token.

## 🛑 Stopping the Server

### Method 1: Kill Process
```zsh
pkill -f "dotnet run"
```

### Method 2: Find and Kill by PID
```zsh
# Find the process
ps aux | grep dotnet

# Kill by process ID
kill -9 <PID>
```

### Method 3: Stop in Terminal
If running in foreground, press **Ctrl+C**

## 🔑 JWT Token Details

- **Algorithm:** HS256 (HMAC SHA256)
- **Access Token Expiry:** 15 minutes
- **Refresh Token Expiry:** 7 days
- **Claims:** sub (user ID), email, username, clientId, roles, exp, iss, aud

## 🗄️ Database Schema

**Users Table**
- Id (UUID, Primary Key)
- Email (string, unique)
- Username (string, unique)
- PasswordHash (string)
- FirstName, LastName (string)
- CreatedAt (timestamp)

**Roles Table**
- Id (UUID)
- Name (Admin, User)
- Description (string)

**UserRoles Table** (Junction)
- UserId (UUID, FK → Users)
- RoleId (UUID, FK → Roles)

**RefreshTokens Table**
- Id (UUID)
- UserId (UUID, FK → Users)
- Token (string, unique)
- ExpiresAt (timestamp)
- CreatedAt, CreatedByIp (string)
- RevokedAt, RevokedByIp (nullable)
- ReplacedByToken (nullable)

## 🔒 Security Features

- ✅ Password hashing with SHA256
- ✅ JWT with HS256 signature verification
- ✅ Refresh token rotation on new access tokens
- ✅ Token revocation on logout
- ✅ Issuer & Audience validation
- ✅ Lifetime validation with 0 clock skew
- ✅ Role-based access control

## 📝 Environment Configurations

### Development
- Swagger UI enabled at `/swagger`
- Detailed error messages
- Development logging

### Production
- Update JWT secret key to a cryptographically secure value
- Use environment variables for sensitive data
- Disable Swagger UI
- Enable HTTPS

## 🐛 Common Issues

### Database Connection Error
```
InvalidOperationException: Connection string 'DefaultConnection' not found
```
**Solution:** Check appsettings.json has correct PostgreSQL connection string.

### Port Already in Use
```
System.Net.Sockets.SocketException: Address already in use
```
**Solution:** Kill existing process or change port in launchSettings.json

### JWT Token Validation Failed
Ensure:
- JWT secret key matches in appsettings.json
- Token not expired (AccessTokenExpirationMinutes)
- Authorization header format: `Bearer {token}`

## 📦 NuGet Dependencies

- `Microsoft.EntityFrameworkCore` (8.0.2)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.2)
- `System.IdentityModel.Tokens.Jwt` (8.15.0)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.8)
- `Microsoft.AspNetCore.Authorization` (8.0.0)

## 🚀 Next Steps

- [ ] Implement Currency Exchange endpoints
- [ ] Add external API integration (e.g., Frankfurter)
- [ ] Implement rate limiting
- [ ] Add request logging & monitoring
- [ ] Deploy to production environment
- [ ] Add unit & integration tests

## 📄 License

Internal project - All rights reserved

## 👤 Contact

For questions or issues, contact the development team.

---

**Last Updated:** January 18, 2026
**Status:** ✅ Authentication System Complete & Tested
