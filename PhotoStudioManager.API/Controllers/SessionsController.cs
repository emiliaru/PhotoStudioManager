using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoStudioManager.Core.Entities;
using PhotoStudioManager.Infrastructure.Data;
using PhotoStudioManager.Application.DTOs.Sessions;
using System.Security.Claims;

namespace PhotoStudioManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SessionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessions()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Get user's role
        var isPhotographer = User.IsInRole("Photographer");
        var isClient = User.IsInRole("Client");

        var query = _context.PhotoSessions
            .Include(s => s.Client)
            .Include(s => s.Photographer)
            .Include(s => s.Photos)
            .AsQueryable();

        // Filter based on role
        if (isPhotographer)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (photographer == null)
            {
                return NotFound("Photographer profile not found");
            }
            query = query.Where(s => s.PhotographerId == photographer.Id);
        }
        else if (isClient)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null)
            {
                return NotFound("Client profile not found");
            }
            query = query.Where(s => s.ClientId == client.Id);
        }

        var sessions = await query
            .Select(s => new SessionDto
            {
                Id = s.Id,
                ClientName = $"{s.Client.FirstName} {s.Client.LastName}",
                PhotographerName = $"{s.Photographer.FirstName} {s.Photographer.LastName}",
                Date = s.Date,
                Status = s.Status.ToString(),
                PhotoCount = s.Photos.Count,
                Description = s.Description,
                Location = s.Location,
                Price = s.Price
            })
            .ToListAsync();

        return Ok(sessions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessionDto>> GetSession(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var session = await _context.PhotoSessions
            .Include(s => s.Client)
            .Include(s => s.Photographer)
            .Include(s => s.Photos)
            .Include(s => s.RequiredEquipment)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
        {
            return NotFound();
        }

        // Check if user has access to this session
        var isPhotographer = User.IsInRole("Photographer");
        var isClient = User.IsInRole("Client");

        if (isPhotographer)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (photographer == null || session.PhotographerId != photographer.Id)
            {
                return Forbid();
            }
        }
        else if (isClient)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || session.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        var sessionDto = new SessionDto
        {
            Id = session.Id,
            ClientName = $"{session.Client.FirstName} {session.Client.LastName}",
            PhotographerName = $"{session.Photographer.FirstName} {session.Photographer.LastName}",
            Date = session.Date,
            Status = session.Status.ToString(),
            PhotoCount = session.Photos.Count,
            Description = session.Description,
            Location = session.Location,
            Price = session.Price,
            Photos = session.Photos.Select(p => new PhotoDto
            {
                Id = p.Id,
                Title = p.Title,
                FilePath = p.FilePath,
                UploadedAt = p.UploadedAt,
                IsApproved = p.IsApproved
            }),
            Equipment = session.RequiredEquipment.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Type = e.Type,
                Condition = e.Condition
            })
        };

        return Ok(sessionDto);
    }

    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession(PhotoSession session)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var isPhotographer = User.IsInRole("Photographer");
        var isClient = User.IsInRole("Client");

        if (isPhotographer)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (photographer == null)
            {
                return NotFound("Photographer profile not found");
            }
            session.PhotographerId = photographer.Id;
        }
        else if (isClient)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null)
            {
                return NotFound("Client profile not found");
            }
            session.ClientId = client.Id;
        }

        _context.PhotoSessions.Add(session);
        await _context.SaveChangesAsync();

        var sessionDto = new SessionDto
        {
            Id = session.Id,
            ClientName = $"{session.Client.FirstName} {session.Client.LastName}",
            PhotographerName = $"{session.Photographer.FirstName} {session.Photographer.LastName}",
            Date = session.Date,
            Status = session.Status.ToString(),
            PhotoCount = session.Photos.Count,
            Description = session.Description,
            Location = session.Location,
            Price = session.Price
        };

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, sessionDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSession(int id, PhotoSession session)
    {
        if (id != session.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var existingSession = await _context.PhotoSessions
            .Include(s => s.Client)
            .Include(s => s.Photographer)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (existingSession == null)
        {
            return NotFound();
        }

        // Check if user has access to this session
        var isPhotographer = User.IsInRole("Photographer");
        var isClient = User.IsInRole("Client");

        if (isPhotographer)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (photographer == null || existingSession.PhotographerId != photographer.Id)
            {
                return Forbid();
            }
        }
        else if (isClient)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || existingSession.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        existingSession.Date = session.Date;
        existingSession.Status = session.Status;
        existingSession.Description = session.Description;
        existingSession.Location = session.Location;
        existingSession.Price = session.Price;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SessionExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var isPhotographer = User.IsInRole("Photographer");
        var isClient = User.IsInRole("Client");

        var session = await _context.PhotoSessions
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
        {
            return NotFound();
        }

        if (isPhotographer)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (photographer == null || session.PhotographerId != photographer.Id)
            {
                return Forbid();
            }
        }
        else if (isClient)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || session.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        _context.PhotoSessions.Remove(session);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/photos")]
    public async Task<ActionResult<PhotoDto>> UploadPhoto(int id, IFormFile photo)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var isPhotographer = User.IsInRole("Photographer");
        var isClient = User.IsInRole("Client");

        var session = await _context.PhotoSessions
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
        {
            return NotFound();
        }

        if (isPhotographer)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (photographer == null || session.PhotographerId != photographer.Id)
            {
                return Forbid();
            }
        }
        else if (isClient)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || session.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        // TODO: Implement file upload logic
        var newPhoto = new Photo
        {
            Title = photo.FileName,
            FileName = photo.FileName,
            FilePath = "/uploads/" + photo.FileName, // Temporary path
            UploadedAt = DateTime.UtcNow,
            PhotoSessionId = id
        };

        _context.Photos.Add(newPhoto);
        await _context.SaveChangesAsync();

        var photoDto = new PhotoDto
        {
            Id = newPhoto.Id,
            Title = newPhoto.Title,
            FilePath = newPhoto.FilePath,
            UploadedAt = newPhoto.UploadedAt,
            IsApproved = newPhoto.IsApproved
        };

        return CreatedAtAction(nameof(GetSession), new { id }, photoDto);
    }

    [HttpPut("{sessionId}/photos/{photoId}/approve")]
    public async Task<IActionResult> ApprovePhoto(int sessionId, int photoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var isClient = User.IsInRole("Client");

        if (!isClient)
        {
            return Forbid();
        }

        var photo = await _context.Photos
            .FirstOrDefaultAsync(p => p.PhotoSessionId == sessionId && p.Id == photoId);

        if (photo == null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null || photo.PhotoSession.ClientId != client.Id)
        {
            return Forbid();
        }

        photo.IsApproved = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SessionExists(int id)
    {
        return _context.PhotoSessions.Any(e => e.Id == id);
    }
}
