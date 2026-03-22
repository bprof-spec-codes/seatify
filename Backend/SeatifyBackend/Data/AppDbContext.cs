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
        public DbSet<Sector> Sectors => Set<Sector>();
        public DbSet<Seat> Seats => Set<Seat>();


        public AppDbContext(DbContextOptions<AppDbContext> ctx) : base(ctx)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Auditoriums)
                .WithOne(a => a.Venue)
                .HasForeignKey(a => a.VenueId);

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

            //sector
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
                .OnDelete(DeleteBehavior.SetNull);

            // unique coordinate within matrix
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.MatrixId, s.Row, s.Column })
                .IsUnique();

            // optional useful indexes
            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.MatrixId);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.SectorId);

            //todo: Event, Venue, LayoutMatrix konfigurációk
        }
    }
}
