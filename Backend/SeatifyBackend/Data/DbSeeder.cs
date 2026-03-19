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
                        Id = "venue-id-01",
                        Name = "Budapest Aréna"
                    },
                    new Venue
                    {
                        Id = "venue-id-02",
                        Name = "Debreceni Konferencia Központ"
                    }
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
                        VenueId = venueIds[0],
                        Name = "Nagyterem",
                        Description = "Fő koncertterem",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-02",
                        VenueId = venueIds[0],
                        Name = "Kisterem",
                        Description = "Kisebb rendezvényekhez",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-03",
                        VenueId = venueIds[1],
                        Name = "Konferencia terem A",
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

            if (!ctx.Sectors.Any())
            {
                var sectors = new List<Sector>
                {
                    new Sector
                    {
                        Id = "sector-id-01",
                        AuditoriumId = "aud-id-01",
                        Name = "VIP",
                        Color = "#FFD700",
                        BasePrice = 12000
                    },
                    new Sector
                    {
                        Id = "sector-id-02",
                        AuditoriumId = "aud-id-01",
                        Name = "Standard",
                        Color = "#00FF00",
                        BasePrice = 6000
                    },
                    new Sector
                    {
                        Id = "sector-id-03",
                        AuditoriumId = "aud-id-01",
                        Name = "Balcony",
                        Color = "#0000FF",
                        BasePrice = 8000
                    },

                    new Sector
                    {
                        Id = "sector-id-04",
                        AuditoriumId = "aud-id-02",
                        Name = "Front",
                        Color = "#FF0000",
                        BasePrice = 7000
                    },
                    new Sector
                    {
                        Id = "sector-id-05",
                        AuditoriumId = "aud-id-02",
                        Name = "Back",
                        Color = "#AAAAAA",
                        BasePrice = 4000
                    },

                    new Sector
                    {
                        Id = "sector-id-06",
                        AuditoriumId = "aud-id-03",
                        Name = "Premium",
                        Color = "#FFA500",
                        BasePrice = 9000
                    }
                };

                ctx.Sectors.AddRange(sectors);
                ctx.SaveChanges();
            }
        }
    }
}
