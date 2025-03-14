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
public class CalendarController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CalendarController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("photographer/{photographerId}")]
    public async Task<ActionResult<Calendar>> GetPhotographerCalendar(int photographerId)
    {
        // Verify user has access to this photographer's calendar
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers
            .Include(p => p.Calendar)
            .FirstOrDefaultAsync(p => p.Id == photographerId && p.UserId == userId);

        if (photographer == null)
        {
            return Forbid();
        }

        // Create calendar if it doesn't exist
        if (photographer.Calendar == null)
        {
            photographer.Calendar = new Calendar
            {
                PhotographerId = photographerId,
                Events = new List<CalendarEvent>()
            };
            await _context.SaveChangesAsync();
        }

        var calendar = await _context.Calendars
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.PhotographerId == photographerId);

        if (calendar == null)
        {
            return NotFound();
        }

        return calendar;
    }

    [HttpGet("photographer/{photographerId}/events")]
    public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetPhotographerEvents(
        int photographerId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        // Verify user has access to this photographer's calendar
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers
            .Include(p => p.Calendar)
            .FirstOrDefaultAsync(p => p.Id == photographerId && p.UserId == userId);

        if (photographer == null)
        {
            return Forbid();
        }

        // Create calendar if it doesn't exist
        if (photographer.Calendar == null)
        {
            photographer.Calendar = new Calendar
            {
                PhotographerId = photographerId,
                Events = new List<CalendarEvent>()
            };
            await _context.SaveChangesAsync();
        }

        var events = await _context.CalendarEvents
            .Include(e => e.PhotoSession)
            .Include(e => e.Calendar)
                .ThenInclude(c => c.Photographer)
            .Where(e => e.Calendar.PhotographerId == photographerId &&
                       e.Start >= start &&
                       e.End <= end)
            .ToListAsync();

        return events;
    }

    [HttpPost("photographer/{photographerId}/events")]
    public async Task<ActionResult<CalendarEvent>> CreateEvent(int photographerId, CalendarEvent calendarEvent)
    {
        // Verify user has access to this photographer's calendar
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers
            .Include(p => p.Calendar)
            .FirstOrDefaultAsync(p => p.Id == photographerId && p.UserId == userId);

        if (photographer == null)
        {
            return Forbid();
        }

        // Create calendar if it doesn't exist
        if (photographer.Calendar == null)
        {
            photographer.Calendar = new Calendar
            {
                PhotographerId = photographerId,
                Events = new List<CalendarEvent>()
            };
            await _context.SaveChangesAsync();
        }

        calendarEvent.CalendarId = photographer.Calendar.Id;
        _context.CalendarEvents.Add(calendarEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = calendarEvent.Id }, calendarEvent);
    }

    [HttpGet("events/{id}")]
    public async Task<ActionResult<CalendarEvent>> GetEvent(int id)
    {
        var calendarEvent = await _context.CalendarEvents
            .Include(e => e.PhotoSession)
            .Include(e => e.Calendar)
                .ThenInclude(c => c.Photographer)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (calendarEvent == null)
        {
            return NotFound();
        }

        // Verify user has access to this event
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.Id == calendarEvent.Calendar.PhotographerId && p.UserId == userId);

        if (photographer == null)
        {
            return Forbid();
        }

        return calendarEvent;
    }

    [HttpPut("events/{id}")]
    public async Task<ActionResult<CalendarEvent>> UpdateEvent(int id, CalendarEvent calendarEvent)
    {
        if (id != calendarEvent.Id)
        {
            return BadRequest();
        }

        var existingEvent = await _context.CalendarEvents
            .Include(e => e.Calendar)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent == null)
        {
            return NotFound();
        }

        // Check if the user has permission to update this event
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.Id == existingEvent.Calendar.PhotographerId && p.UserId == userId);

        if (photographer == null)
        {
            return Forbid();
        }

        existingEvent.Title = calendarEvent.Title;
        existingEvent.Description = calendarEvent.Description;
        existingEvent.Start = calendarEvent.Start;
        existingEvent.End = calendarEvent.End;
        existingEvent.Location = calendarEvent.Location;
        existingEvent.Type = calendarEvent.Type;
        existingEvent.PhotoSessionId = calendarEvent.PhotoSessionId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return Ok(existingEvent);
    }

    [HttpDelete("events/{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var calendarEvent = await _context.CalendarEvents
            .Include(e => e.Calendar)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (calendarEvent == null)
        {
            return NotFound();
        }

        // Check if the user has permission to delete this event
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.Id == calendarEvent.Calendar.PhotographerId && p.UserId == userId);

        if (photographer == null)
        {
            return Forbid();
        }

        _context.CalendarEvents.Remove(calendarEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EventExists(int id)
    {
        return _context.CalendarEvents.Any(e => e.Id == id);
    }
}
