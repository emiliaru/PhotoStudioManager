namespace PhotoStudioManager.Core.Entities;

public class Photo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public int PhotoSessionId { get; set; }
    public PhotoSession PhotoSession { get; set; } = null!;

    public ICollection<string> Tags { get; set; } = new List<string>();
}
