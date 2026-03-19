using Entities.Models;

namespace Data
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext ctx)
        {
            if (!ctx.Events.Any())
            {
                var events = new List<Event>
                    {
                        new Event
                        {
                            Id = Guid.NewGuid(),
                            Name = "Teszt esemény 1",
                            Description = "Első seed esemény",
                            StartsAt = DateTime.UtcNow.AddDays(7),
                            EndsAt = DateTime.UtcNow.AddDays(7).AddHours(2),
                            BasePrice = 4990
                        },
                        new Event
                        {
                            Id = Guid.NewGuid(),
                            Name = "Teszt esemény 2",
                            Description = "Második seed esemény",
                            StartsAt = DateTime.UtcNow.AddDays(14),
                            EndsAt = DateTime.UtcNow.AddDays(14).AddHours(3),
                            BasePrice = 6990
                        }
                    };

                ctx.Events.AddRange(events);
                ctx.SaveChanges();
            }
            
            if (!ctx.Venues.Any())
            {
                var venues = new List<Venue>
                {
                    new Venue
                    {
                        Id = "ven1",
                        Name = "Kulturális Fesztivál",
                        City = "Budapest",
                        PostalCode = "1011",
                        AddressLine = "Fő utca 1.",
                        OrganizerId = "org123"
                    },
                    new Venue
                    {
                        Id = "ven2",
                        Name = "Zenei Koncert",
                        City = "Debrecen",
                        PostalCode = "4025",
                        AddressLine = "Kossuth Lajos utca 2.",
                        OrganizerId = "org456"
                    },
                    new Venue
                    {
                        Id = "ven3",
                        Name = "Gasztro Expo",
                        City = "Szeged",
                        PostalCode = "6720",
                        AddressLine = "Klauzál tér 3.",
                        OrganizerId = "org789"
                    },
                };
                
                ctx.Venues.AddRange(venues);
                ctx.SaveChanges();
            }
            
            if (!ctx.Auditoriums.Any())
            {
                var auditoriums = new List<Auditorium>
                {
                    new Auditorium
                    {
                        Id = "aud1",
                        VenueId = "ven1"
                    },
                    new Auditorium
                    {
                        Id = "aud2",
                        VenueId = "ven1"
                    },
                    new Auditorium
                    {
                        Id = "aud3",
                        VenueId = "ven3"
                    },
                    new Auditorium
                    {
                        Id = "aud4",
                        VenueId = "ven3"
                    }
                };
                
                ctx.Auditoriums.AddRange(auditoriums);
                ctx.SaveChanges();
            }
        }
    }
}
