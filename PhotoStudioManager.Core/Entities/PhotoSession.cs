namespace PhotoStudioManager.Core.Entities;

public class PhotoSession
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int PhotographerId { get; set; }
    public DateTime Date { get; set; }
    public SessionStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Client Client { get; set; } = null!;
    public Photographer Photographer { get; set; } = null!;
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<Equipment> RequiredEquipment { get; set; } = new List<Equipment>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public CalendarEvent? CalendarEvent { get; set; }
}

public enum SessionStatus
{
    Scheduled,
    Pending,
    Confirmed,
    Completed,
    Cancelled
}
