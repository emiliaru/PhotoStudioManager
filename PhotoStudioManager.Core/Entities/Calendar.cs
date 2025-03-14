using System.Collections.Generic;

namespace PhotoStudioManager.Core.Entities;

public class Calendar
{
    public int Id { get; set; }
    public int PhotographerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }

    // Navigation properties
    public Photographer Photographer { get; set; } = null!;
    public ICollection<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
}

public class CalendarEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Location { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public int CalendarId { get; set; }
    public Calendar Calendar { get; set; } = null!;
    public int? PhotoSessionId { get; set; }
    public PhotoSession? PhotoSession { get; set; }
}

public enum EventType
{
    PhotoSession,
    Meeting,
    Other
}
