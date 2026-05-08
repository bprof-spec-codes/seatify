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
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Organizer> Organizers => Set<Organizer>();
        public DbSet<Appearance> Appearances => Set<Appearance>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();
        public DbSet<EventSeatOverride> EventSeatOverrides => Set<EventSeatOverride>();
        public DbSet<OccurrenceSeatOverride> OccurrenceSeatOverrides => Set<OccurrenceSeatOverride>();
        public DbSet<SeatHold> seatHolds => Set<SeatHold>();
        public DbSet<BookingSession> bookingSessions => Set<BookingSession>();

        public AppDbContext(DbContextOptions<AppDbContext> ctx) : base(ctx)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // venue
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Auditoriums)
                .WithOne(a => a.Venue)
                .HasForeignKey(a => a.VenueId);

            // auditorium
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

            // sector
            modelBuilder.Entity<Sector>()
                .HasOne(s => s.Auditorium)
                .WithMany(a => a.Sectors)
                .HasForeignKey(s => s.AuditoriumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sector>()
                .HasIndex(s => new { s.AuditoriumId, s.Name })
                .IsUnique();

            // seat -> layout matrix
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.LayoutMatrix)
                .WithMany(m => m.Seats)
                .HasForeignKey(s => s.MatrixId)
                .OnDelete(DeleteBehavior.Cascade);

            // seat -> sector
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Sector)
                .WithMany(sec => sec.Seats)
                .HasForeignKey(s => s.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // unique coordinate within matrix
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.MatrixId, s.Row, s.Column })
                .IsUnique();

            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.MatrixId);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.SectorId);

            // organizer
            modelBuilder.Entity<Organizer>()
                .HasIndex(o => o.Email)
                .IsUnique();

            // layout matrix
            modelBuilder.Entity<LayoutMatrix>()
                .HasIndex(lm => new { lm.AuditoriumId, lm.Name })
                .IsUnique();

            modelBuilder.Entity<LayoutMatrix>()
                .HasIndex(lm => lm.AuditoriumId);

            // appearance
            modelBuilder.Entity<Appearance>()
                .HasOne(a => a.Organizer)
                .WithMany(o => o.Appearances)
                .HasForeignKey(a => a.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);

            // event appearance
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Appearance)
                .WithMany()
                .HasForeignKey(e => e.AppearanceId)
                .OnDelete(DeleteBehavior.SetNull);

            // event occurrence appearance
            modelBuilder.Entity<EventOccurrence>()
                .HasOne(eo => eo.Appearance)
                .WithMany()
                .HasForeignKey(eo => eo.AppearanceId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // EventSeatOverride
            modelBuilder.Entity<EventSeatOverride>()
                .HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventSeatOverride>()
                .HasOne(e => e.Seat)
                .WithMany()
                .HasForeignKey(e => e.SeatId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventSeatOverride>()
                .HasOne(e => e.Sector)
                .WithMany()
                .HasForeignKey(e => e.SectorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<EventSeatOverride>()
                .HasIndex(e => new { e.EventId, e.SeatId })
                .IsUnique();

            // OccurrenceSeatOverride
            modelBuilder.Entity<OccurrenceSeatOverride>()
                .HasOne(o => o.Occurrence)
                .WithMany()
                .HasForeignKey(o => o.OccurrenceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OccurrenceSeatOverride>()
                .HasOne(o => o.Seat)
                .WithMany()
                .HasForeignKey(o => o.SeatId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OccurrenceSeatOverride>()
                .HasOne(o => o.Sector)
                .WithMany()
                .HasForeignKey(o => o.SectorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<OccurrenceSeatOverride>()
                .HasIndex(o => new { o.OccurrenceId, o.SeatId })
                .IsUnique();

            modelBuilder.Entity<BookingSession>()
                .HasOne(b => b.EventOccurrence)
                .WithMany()
                .HasForeignKey(b => b.EventOccurrendeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SeatHold>()
                .HasOne(s => s.BookingSession)
                .WithMany()
                .HasForeignKey(s => s.BookingSessionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SeatHold>()
                .HasOne(s => s.EventOccurrence)
                .WithMany()
                .HasForeignKey(s => s.EventOccurrenceId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SeatHold>()
                .HasOne(s => s.Seat)
                .WithMany()
                .HasForeignKey(s => s.SeatId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
