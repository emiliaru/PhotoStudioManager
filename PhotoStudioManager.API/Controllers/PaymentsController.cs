using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoStudioManager.Core.Entities;
using PhotoStudioManager.Infrastructure.Data;
using PhotoStudioManager.Infrastructure.Services;

namespace PhotoStudioManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public PaymentsController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
    {
        return await _context.Payments
            .Include(p => p.PhotoSession)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPayment(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.PhotoSession)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        return payment;
    }

    [HttpPost]
    public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
    {
        payment.CreatedAt = DateTime.UtcNow;
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var session = await _context.PhotoSessions
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == payment.PhotoSessionId);

        if (session?.Client?.Email != null)
        {
            await _emailService.SendPaymentReminderAsync(
                session.Client.Email,
                payment.Amount,
                session.Title);
        }

        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentStatus status)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
        {
            return NotFound();
        }

        payment.Status = status;
        if (status == PaymentStatus.Completed)
        {
            payment.PaidAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetSessionPayments(int sessionId)
    {
        return await _context.Payments
            .Where(p => p.PhotoSessionId == sessionId)
            .ToListAsync();
    }

    [HttpPost("{id}/invoice")]
    public async Task<ActionResult<Invoice>> GenerateInvoice(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.PhotoSession)
            .ThenInclude(s => s.Client)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        var invoice = new Invoice
        {
            PaymentId = payment.Id,
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{payment.Id}",
            IssuedDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            BillingAddress = payment.PhotoSession?.Client?.Address ?? "",
            TaxRate = 0.23m, // 23% VAT
            TaxAmount = payment.Amount * 0.23m,
            TotalAmount = payment.Amount * 1.23m
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    [HttpGet("invoice/{id}")]
    public async Task<ActionResult<Invoice>> GetInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Payment)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
        {
            return NotFound();
        }

        return invoice;
    }
}
