using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<Auditorium> Auditoriums => Set<Auditorium>();
        public DbSet<LayoutMatrix> LayoutMatrices => Set<LayoutMatrix>();
        public DbSet<EventOccurrence> EventOccurrences => Set<EventOccurrence>();
        public DbSet<Sector> Sectors => Set<Sector>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();

        public AppDbContext(DbContextOptions<AppDbContext> ctx) : base(ctx)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //auditorium
            modelBuilder.Entity<Auditorium>()
                .HasMany(a => a.LayoutMatrices)
                .WithOne(lm => lm.Auditorium)
                .HasForeignKey(lm => lm.AuditoriumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Auditorium>()
                .HasOne(a => a.Venue)
                .WithMany(v => v.Auditoriums)
                .HasForeignKey(a => a.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            // EventOccurrence
            modelBuilder.Entity<EventOccurrence>()
                .HasOne(e => e.Event)
                .WithMany(e => e.EventOccurrences) 
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventOccurrence>()
                .HasOne(e => e.Auditorium)
                .WithMany(a => a.EventOccurrences)
                .HasForeignKey(e => e.AuditoriumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventOccurrence>()
                .HasOne(e => e.Venue)
                .WithMany()
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.NoAction);

            //sector
            modelBuilder.Entity<Sector>()
                .HasOne(s => s.Auditorium)
                .WithMany(a => a.Sectors)
                .HasForeignKey(s => s.AuditoriumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sector>()
                .HasIndex(s => new { s.AuditoriumId, s.Name })
                .IsUnique();

            //todo: Event, Venue, LayoutMatrix konfigurációk

            // Reservation
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.EventOccurrence)
                .WithMany(e => e.Reservations)
                .HasForeignKey(r => r.EventOccurrenceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationSeat>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationSeats)
                .HasForeignKey(rs => rs.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
