using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoStudioManager.Core.Entities;
using PhotoStudioManager.Infrastructure.Data;
using System.Security.Claims;

namespace PhotoStudioManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PhotographersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PhotographersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetPhotographers()
    {
        var photographers = await _context.Photographers
            .Include(p => p.User)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.Phone,
                p.Specialization,
                p.HourlyRate,
                p.Portfolio,
                Name = $"{p.FirstName} {p.LastName}"
            })
            .ToListAsync();

        return photographers;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<object>> GetPhotographerByUserId(string userId)
    {
        var photographer = await _context.Photographers
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.Phone,
                p.Specialization,
                p.HourlyRate,
                p.Portfolio,
                Name = $"{p.FirstName} {p.LastName}"
            })
            .FirstOrDefaultAsync();

        if (photographer == null)
        {
            return NotFound();
        }

        return photographer;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetPhotographer(int id)
    {
        var photographer = await _context.Photographers
            .Include(p => p.User)
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.Phone,
                p.Specialization,
                p.HourlyRate,
                p.Portfolio,
                Name = $"{p.FirstName} {p.LastName}"
            })
            .FirstOrDefaultAsync();

        if (photographer == null)
        {
            return NotFound();
        }

        return photographer;
    }

    [HttpPost]
    public async Task<ActionResult<Photographer>> CreatePhotographer(Photographer photographer)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        photographer.UserId = userId;
        photographer.CreatedAt = DateTime.UtcNow;

        _context.Photographers.Add(photographer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPhotographer), new { id = photographer.Id }, photographer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePhotographer(int id, Photographer photographer)
    {
        if (id != photographer.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var existingPhotographer = await _context.Photographers.FindAsync(id);

        if (existingPhotographer == null)
        {
            return NotFound();
        }

        if (existingPhotographer.UserId != userId)
        {
            return Forbid();
        }

        existingPhotographer.FirstName = photographer.FirstName;
        existingPhotographer.LastName = photographer.LastName;
        existingPhotographer.Email = photographer.Email;
        existingPhotographer.Phone = photographer.Phone;
        existingPhotographer.Specialization = photographer.Specialization;
        existingPhotographer.HourlyRate = photographer.HourlyRate;
        existingPhotographer.Portfolio = photographer.Portfolio;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PhotographerExists(id))
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
    public async Task<IActionResult> DeletePhotographer(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers.FindAsync(id);

        if (photographer == null)
        {
            return NotFound();
        }

        if (photographer.UserId != userId)
        {
            return Forbid();
        }

        _context.Photographers.Remove(photographer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PhotographerExists(int id)
    {
        return _context.Photographers.Any(e => e.Id == id);
    }
}
