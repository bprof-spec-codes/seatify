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
                            Id = "event-id-01",
                            Name = "Teszt esemény 1",
                            Description = "Első seed esemény",
                            StartsAt = DateTime.UtcNow.AddDays(7),
                            EndsAt = DateTime.UtcNow.AddDays(7).AddHours(2),
                            BasePrice = 4990
                        },
                        new Event
                        {
                            Id = "event-id-02",
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
                        Id = "ven-id-01",
                        Name = "Kulturális Fesztivál",
                        City = "Budapest",
                        PostalCode = "1011",
                        AddressLine = "Fő utca 1.",
                        OrganizerId = "org123"
                    },
                    new Venue
                    {
                        Id = "ven-id-02",
                        Name = "Zenei Koncert",
                        City = "Debrecen",
                        PostalCode = "4025",
                        AddressLine = "Kossuth Lajos utca 2.",
                        OrganizerId = "org456"
                    },
                    new Venue
                    {
                        Id = "ven-id-03",
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
                var venueIds = ctx.Venues.Select(v => v.Id).ToList();

                var auditoriums = new List<Auditorium>
                {
                    new Auditorium
                    {
                        Id = "aud-id-01",
                        VenueId = "ven-id-01",
                        Name = "Nagyterem",
                        Description = "Fő koncertterem",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-02",
                        VenueId = "ven-id-01",
                        Name = "Kisterem",
                        Description = "Kisebb rendezvényekhez",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-03",
                        VenueId = "ven-id-03",
                        Name = "Konferencia terem A",
                        Description = "Konferenciákhoz",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-04",
                        VenueId = "ven-id-03",
                        Name = "Konferencia terem B",
                        Description = "Konferenciákhoz",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                };
                
                ctx.Auditoriums.AddRange(auditoriums);
                ctx.SaveChanges();
            }
            
            if (!ctx.LayoutMatrices.Any())
            {
                var layoutMatrices = new List<LayoutMatrix>
                {
                    new LayoutMatrix
                    {
                        Id = "matrix-id-01",
                        AuditoriumId = "aud-id-01"
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-02",
                        AuditoriumId = "aud-id-02"
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-03",
                        AuditoriumId = "aud-id-03"
                    }
                };

                ctx.LayoutMatrices.AddRange(layoutMatrices);
                ctx.SaveChanges();
            }
        }
    }
}
