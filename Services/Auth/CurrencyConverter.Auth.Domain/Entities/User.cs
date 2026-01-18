namespace CurrencyConverter.Auth.Domain.Entities;
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    private User() {}

    public static User Create(string email, string username, string passwordHash, string firstName, string lastName)
    {
        // Validation
        if(string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if(string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if(string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
        
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void Activate()
    {
        IsActive = true;
    }

    public void AddRole(Role role)
    {
        if (UserRoles.Any(ur => ur.RoleId == role.Id))
            return; // Already has this role
        
        UserRoles.Add(new UserRole
        {
            UserId = Id,
            RoleId = role.Id
        });
    }
}
