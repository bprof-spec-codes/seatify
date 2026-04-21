using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Data
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext ctx)
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            if (!ctx.Organizers.Any())
            {
                var passwordHasher = new PasswordHasher<Organizer>();

                var devOrganizer = new Organizer
                {
                    Id = "org-id-01",
                    Name = "Dev Organizer",
                    Email = "dev@seatify.hu",
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                devOrganizer.PasswordHash = passwordHasher.HashPassword(devOrganizer, "123456789");

                var organizers = new List<Organizer>
                {
                    devOrganizer
                };

                ctx.Organizers.AddRange(organizers);
                ctx.SaveChanges();
            }

            if (!ctx.Events.Any())
            {
                var events = new List<Event>
                {
                    new Event
                    {
                        Id = "event-01",
                        OrganizerId = "org-id-01",
                        Slug = "rock-festival-2026",
                        Name = "Rock Fesztivál 2026",
                        Description = "A legnagyobb rock esemény az országban.",
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Event
                    {
                        Id = "event-02",
                        OrganizerId = "org-id-01",
                        Slug = "classical-gala",
                        Name = "Klasszikus Gálaest",
                        Description = "Világhírű zeneszerzők művei egy este alatt.",
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
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
                        Name = "Budapesti Aréna",
                        City = "Budapest",
                        PostalCode = "1143",
                        AddressLine = "Stefánia út 2.",
                        OrganizerId = "org-id-01"
                    },
                    new Venue
                    {
                        Id = "ven-id-02",
                        Name = "Művészetek Palotája",
                        City = "Budapest",
                        PostalCode = "1095",
                        AddressLine = "Komor Marcell u. 1.",
                        OrganizerId = "org-id-01"
                    }
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
                        Id = "aud-id-01",
                        VenueId = "ven-id-01",
                        Name = "Main Hall",
                        Description = "Nagyszínpados aréna",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-02",
                        VenueId = "ven-id-01",
                        Name = "B-Stage Hall",
                        Description = "Kisebb rendezvényekhez az Arénában",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-03",
                        VenueId = "ven-id-02",
                        Name = "Bartók Béla Nemzeti Hangversenyterem",
                        Description = "Világszínvonalú akusztika",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Auditorium
                    {
                        Id = "aud-id-04",
                        VenueId = "ven-id-02",
                        Name = "Fesztivál Színház",
                        Description = "Modern színházterem",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                };

                ctx.Auditoriums.AddRange(auditoriums);
                ctx.SaveChanges();
            }

            if (!ctx.LayoutMatrices.Any())
            {
                var now = DateTime.UtcNow;

                var layoutMatrices = new List<LayoutMatrix>
                {
                    new LayoutMatrix
                    {
                        Id = "matrix-id-01",
                        AuditoriumId = "aud-id-01",
                        Name = "Arena Main Grid",
                        Rows = 15,
                        Columns = 20,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-02",
                        AuditoriumId = "aud-id-02",
                        Name = "B-Stage Grid",
                        Rows = 10,
                        Columns = 12,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-03",
                        AuditoriumId = "aud-id-03",
                        Name = "Bartók Grid",
                        Rows = 20,
                        Columns = 25,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-04",
                        AuditoriumId = "aud-id-04",
                        Name = "Theater Grid",
                        Rows = 12,
                        Columns = 15,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    }
                };

                ctx.LayoutMatrices.AddRange(layoutMatrices);
                ctx.SaveChanges();
            }

            if (!ctx.Sectors.Any())
            {
                var sectors = new List<Sector>
                {
                    new Sector { Id = "sector-01", AuditoriumId = "aud-id-01", Name = "Front VIP", Color = "#FFD700", BasePrice = 25000 },
                    new Sector { Id = "sector-02", AuditoriumId = "aud-id-01", Name = "Arena Standing", Color = "#22c55e", BasePrice = 12000 },
                    new Sector { Id = "sector-03", AuditoriumId = "aud-id-01", Name = "Side Seating", Color = "#3b82f6", BasePrice = 15000 },

                    new Sector { Id = "sector-04", AuditoriumId = "aud-id-03", Name = "Parterre", Color = "#991b1b", BasePrice = 18000 },
                    new Sector { Id = "sector-05", AuditoriumId = "aud-id-03", Name = "First Balcony", Color = "#1e40af", BasePrice = 14000 },
                    new Sector { Id = "sector-06", AuditoriumId = "aud-id-04", Name = "Theater Center", Color = "#6d28d9", BasePrice = 10000 }
                };

                ctx.Sectors.AddRange(sectors);
                ctx.SaveChanges();
            }

            if (!ctx.Seats.Any())
            {
                var now = DateTime.UtcNow;
                var seats = new List<Seat>();
                var matrices = ctx.LayoutMatrices.ToList();

                foreach (var matrix in matrices)
                {
                    for (int row = 1; row <= matrix.Rows; row++)
                    {
                        for (int column = 1; column <= matrix.Columns; column++)
                        {
                            var seat = new Seat
                            {
                                Id = Guid.NewGuid().ToString(),
                                MatrixId = matrix.Id,
                                Row = row,
                                Column = column,
                                SeatLabel = $"{GetRowLabel(row)}{column}",
                                SeatType = SeatType.Seat,
                                CreatedAtUtc = now,
                                UpdatedAtUtc = now
                            };

                            if (matrix.Id == "matrix-id-01")
                            {
                                if (row <= 3) seat.SectorId = "sector-01";
                                else if (row <= 10) seat.SectorId = "sector-02";
                                else seat.SectorId = "sector-03";
                            }
                            else if (matrix.Id == "matrix-id-03")
                            {
                                if (row <= 10) seat.SectorId = "sector-04";
                                else seat.SectorId = "sector-05";
                            }
                            else if (matrix.Id == "matrix-id-04")
                            {
                                seat.SectorId = "sector-06";
                            }

                            seats.Add(seat);
                        }
                    }
                }

                ctx.Seats.AddRange(seats);
                ctx.SaveChanges();
            }

            if (!ctx.EventOccurrences.Any())
            {
                var occurrences = new List<EventOccurrence>
                {
                    new EventOccurrence
                    {
                        Id = "occ-01",
                        EventId = "event-01",
                        VenueId = "ven-id-01",
                        AuditoriumId = "aud-id-01",
                        StartsAtUtc = DateTime.UtcNow.AddDays(30),
                        EndsAtUtc = DateTime.UtcNow.AddDays(30).AddHours(5),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(30),
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new EventOccurrence
                    {
                        Id = "occ-02",
                        EventId = "event-01",
                        VenueId = "ven-id-01",
                        AuditoriumId = "aud-id-02",
                        StartsAtUtc = DateTime.UtcNow.AddDays(31),
                        EndsAtUtc = DateTime.UtcNow.AddDays(31).AddHours(4),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(31),
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new EventOccurrence
                    {
                        Id = "occ-03",
                        EventId = "event-02",
                        VenueId = "ven-id-02",
                        AuditoriumId = "aud-id-03",
                        StartsAtUtc = DateTime.UtcNow.AddDays(45),
                        EndsAtUtc = DateTime.UtcNow.AddDays(45).AddHours(3),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(45),
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new EventOccurrence
                    {
                        Id = "occ-04",
                        EventId = "event-02",
                        VenueId = "ven-id-02",
                        AuditoriumId = "aud-id-04",
                        StartsAtUtc = DateTime.UtcNow.AddDays(46),
                        EndsAtUtc = DateTime.UtcNow.AddDays(46).AddHours(3),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(46),
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                };

                ctx.EventOccurrences.AddRange(occurrences);
                ctx.SaveChanges();
            }
        }

        private static string GetRowLabel(int rowNumber)
        {
            var label = string.Empty;

            while (rowNumber > 0)
            {
                rowNumber--;
                label = (char)('A' + (rowNumber % 26)) + label;
                rowNumber /= 26;
            }

            return label;
        }
    }
}
