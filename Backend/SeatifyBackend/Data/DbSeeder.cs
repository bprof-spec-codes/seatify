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
                var now = DateTime.UtcNow;

                var layoutMatrices = new List<LayoutMatrix>
                {
                    new LayoutMatrix
                    {
                        Id = "matrix-id-01",
                        AuditoriumId = "aud-id-01",
                        Name = "Main Floor",
                        Rows = 10,
                        Columns = 12,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-02",
                        AuditoriumId = "aud-id-01",
                        Name = "Balcony",
                        Rows = 5,
                        Columns = 10,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-03",
                        AuditoriumId = "aud-id-02",
                        Name = "Standard Layout",
                        Rows = 8,
                        Columns = 10,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-04",
                        AuditoriumId = "aud-id-03",
                        Name = "Conference Left Block",
                        Rows = 6,
                        Columns = 8,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-05",
                        AuditoriumId = "aud-id-03",
                        Name = "Conference Right Block",
                        Rows = 6,
                        Columns = 8,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new LayoutMatrix
                    {
                        Id = "matrix-id-06",
                        AuditoriumId = "aud-id-04",
                        Name = "Default Layout",
                        Rows = 7,
                        Columns = 9,
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
                            seats.Add(new Seat
                            {
                                Id = Guid.NewGuid().ToString(),
                                MatrixId = matrix.Id,
                                Row = row,
                                Column = column,
                                SeatLabel = $"{GetRowLabel(row)}{column}",
                                SectorId = null, // később assignolható
                                SeatType = SeatType.Seat,
                                PriceOverride = null,
                                CreatedAtUtc = now,
                                UpdatedAtUtc = now
                            });
                        }
                    }
                }

                ctx.Seats.AddRange(seats);
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
