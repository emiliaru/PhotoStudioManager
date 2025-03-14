using Microsoft.AspNetCore.Identity;

namespace PhotoStudioManager.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Client? Client { get; set; }
    public Photographer? Photographer { get; set; }
}
