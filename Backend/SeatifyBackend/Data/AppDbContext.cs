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

        public AppDbContext(DbContextOptions<AppDbContext> ctx) : base(ctx)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            //todo: Event, Venue, LayoutMatrix konfigurációk
        }
    }
}
