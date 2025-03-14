using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhotoStudioManager.Core.Entities;

namespace PhotoStudioManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Photographer> Photographers { get; set; } = null!;
    public DbSet<PhotoSession> PhotoSessions { get; set; } = null!;
    public DbSet<Photo> Photos { get; set; } = null!;
    public DbSet<Equipment> Equipment { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Calendar> Calendars { get; set; } = null!;
    public DbSet<CalendarEvent> CalendarEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed roles
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Photographer",
                NormalizedName = "PHOTOGRAPHER"
            },
            new IdentityRole
            {
                Id = "3",
                Name = "Client",
                NormalizedName = "CLIENT"
            }
        );

        // Configure Identity tables
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(name: "AspNetUsers");
        });

        // Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Photographer configuration
        modelBuilder.Entity<Photographer>(entity =>
        {
            entity.HasOne(p => p.User)
                .WithOne(u => u.Photographer)
                .HasForeignKey<Photographer>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Calendars)
                .WithOne(c => c.Photographer)
                .HasForeignKey(c => c.PhotographerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PhotoSession configuration
        modelBuilder.Entity<PhotoSession>(entity =>
        {
            entity.HasOne(s => s.Client)
                .WithMany(c => c.PhotoSessions)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Photographer)
                .WithMany(p => p.PhotoSessions)
                .HasForeignKey(s => s.PhotographerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.CalendarEvent)
                .WithOne(e => e.PhotoSession)
                .HasForeignKey<CalendarEvent>(e => e.PhotoSessionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Photo configuration
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasOne(p => p.PhotoSession)
                .WithMany(s => s.Photos)
                .HasForeignKey(p => p.PhotoSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Equipment configuration
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasMany(e => e.PhotoSessions)
                .WithMany(s => s.RequiredEquipment)
                .UsingEntity(j => j.ToTable("EquipmentPhotoSession"));
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasOne(p => p.PhotoSession)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasOne(i => i.Payment)
                .WithMany()
                .HasForeignKey(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(i => i.TaxRate)
                .HasColumnType("decimal(5,2)");

            entity.Property(i => i.TaxAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(i => i.TotalAmount)
                .HasColumnType("decimal(18,2)");
        });

        // Calendar configuration
        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasMany(c => c.Events)
                .WithOne(e => e.Calendar)
                .HasForeignKey(e => e.CalendarId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Description)
                .HasMaxLength(500);

            entity.Property(c => c.Color)
                .HasMaxLength(7);
        });

        // CalendarEvent configuration
        modelBuilder.Entity<CalendarEvent>(entity =>
        {
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Location)
                .HasMaxLength(200);

            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsRequired();
        });

        // Add example data
        modelBuilder.Entity<Client>().HasData(
            new Client 
            { 
                Id = 1, 
                FirstName = "Anna", 
                LastName = "Kowalska", 
                Email = "anna@example.com", 
                PhoneNumber = "123456789", 
                CreatedAt = DateTime.UtcNow 
            },
            new Client 
            { 
                Id = 2, 
                FirstName = "Jan", 
                LastName = "Nowak", 
                Email = "jan@example.com", 
                PhoneNumber = "987654321", 
                CreatedAt = DateTime.UtcNow 
            }
        );

        modelBuilder.Entity<PhotoSession>().HasData(
            new PhotoSession 
            { 
                Id = 1, 
                ClientId = 1, 
                PhotographerId = 1, 
                Date = DateTime.UtcNow.AddDays(7), 
                Status = SessionStatus.Scheduled,
                Description = "Sesja ślubna w plenerze",
                Location = "Park Miejski",
                Price = 1500M,
                CreatedAt = DateTime.UtcNow
            },
            new PhotoSession 
            { 
                Id = 2, 
                ClientId = 2, 
                PhotographerId = 1, 
                Date = DateTime.UtcNow.AddDays(14), 
                Status = SessionStatus.Scheduled,
                Description = "Sesja rodzinna",
                Location = "Studio",
                Price = 800M,
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Calendar>().HasData(
            new Calendar { Id = 1, PhotographerId = 1 }
        );

        modelBuilder.Entity<CalendarEvent>().HasData(
            new CalendarEvent 
            { 
                Id = 1, 
                CalendarId = 1, 
                PhotoSessionId = 1,
                Title = "Sesja ślubna - Anna Kowalska",
                Description = "Sesja ślubna w plenerze",
                Start = DateTime.UtcNow.AddDays(7).AddHours(10),
                End = DateTime.UtcNow.AddDays(7).AddHours(14),
                Location = "Park Miejski",
                Type = EventType.PhotoSession
            },
            new CalendarEvent 
            { 
                Id = 2, 
                CalendarId = 1, 
                PhotoSessionId = 2,
                Title = "Sesja rodzinna - Jan Nowak",
                Description = "Sesja rodzinna w studio",
                Start = DateTime.UtcNow.AddDays(14).AddHours(15),
                End = DateTime.UtcNow.AddDays(14).AddHours(17),
                Location = "Studio",
                Type = EventType.PhotoSession
            }
        );
    }
}
