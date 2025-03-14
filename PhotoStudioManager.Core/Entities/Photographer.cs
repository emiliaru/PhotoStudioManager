using System;
using System.Collections.Generic;

namespace PhotoStudioManager.Core.Entities;

public class Photographer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Website { get; set; }
    public string? Portfolio { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ApplicationUser? User { get; set; }
    public ICollection<PhotoSession> PhotoSessions { get; set; } = new List<PhotoSession>();
    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();
}
