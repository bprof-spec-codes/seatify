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
        }
    }
}
