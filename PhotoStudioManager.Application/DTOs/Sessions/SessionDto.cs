using System;
using System.Collections.Generic;

namespace PhotoStudioManager.Application.DTOs.Sessions;

public class SessionDto
{
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string PhotographerName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PhotoCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public IEnumerable<PhotoDto> Photos { get; set; } = new List<PhotoDto>();
    public IEnumerable<EquipmentDto> Equipment { get; set; } = new List<EquipmentDto>();
}

public class PhotoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public bool IsApproved { get; set; }
}

public class EquipmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
}
