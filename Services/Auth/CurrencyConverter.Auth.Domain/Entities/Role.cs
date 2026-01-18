namespace CurrencyConverter.Auth.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    
    private Role() { }
    
    public static Role Create(string name, string description)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }
    
    // Predefined roles
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}