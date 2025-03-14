using PhotoStudioManager.Core.Entities;

namespace PhotoStudioManager.Application.DTOs.Auth;

public class RegisterDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
    
    // Dodatkowe pola dla fotografa
    public string? Specialization { get; set; }
    public decimal? HourlyRate { get; set; }
    
    // Dodatkowe pola dla klienta
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
