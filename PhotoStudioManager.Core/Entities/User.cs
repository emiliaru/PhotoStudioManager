namespace PhotoStudioManager.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    
    // Dla fotografów
    public Photographer? Photographer { get; set; }
    // Dla klientów
    public Client? Client { get; set; }
}

public enum UserRole
{
    Admin,
    Photographer,
    Client
}
