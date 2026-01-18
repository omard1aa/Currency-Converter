using Microsoft.EntityFrameworkCore;
using CurrencyConverter.Auth.Domain.Entities;

namespace CurrencyConverter.Auth.Infrastructure.Persistence;
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.PasswordHash)
                .IsRequired();
            
            entity.HasIndex(e => e.Email)
                .IsUnique();
            
            entity.HasIndex(e => e.Username)
                .IsUnique();
            
            // Relationships
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(e => e.Name)
                .IsUnique();
        });
        
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });
        
        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Token)
                .IsRequired();
            
            entity.HasIndex(e => e.Token);
        });
        
        // Seed initial roles
        SeedRoles(modelBuilder);
    }
    
    private void SeedRoles(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        
        modelBuilder.Entity<Role>().HasData(
            new
            {
                Id = adminRoleId,
                Name = Role.Roles.Admin,
                Description = "Administrator with full access"
            },
            new
            {
                Id = userRoleId,
                Name = Role.Roles.User,
                Description = "Standard user with basic access"
            }
        );
    }
}