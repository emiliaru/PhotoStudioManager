using System;

namespace PhotoStudioManager.Core.Entities;

public class CalendarEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Location { get; set; }
    public string Type { get; set; } = string.Empty;
    public int PhotographerId { get; set; }
    public int CalendarId { get; set; }
    public int? PhotoSessionId { get; set; }
    public decimal? Price { get; set; }
    public int? PhotoCount { get; set; }

    // Navigation properties
    public Calendar Calendar { get; set; } = null!;
    public PhotoSession? PhotoSession { get; set; }
    public Photographer Photographer { get; set; } = null!;
}
