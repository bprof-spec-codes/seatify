using Entities.Models;

namespace Data
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext ctx)
        {
            if (!ctx.Organizers.Any())
            {
                var organizers = new List<Organizer>
                {
                    new Organizer
                    {
                        Id = "org123",
                        Name = "Budapest Event Organizers",
                        Email = "contact@budapest-events.hu",
                        Password = "HashedPassword123!",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Organizer
                    {
                        Id = "org456",
                        Name = "Debrecen Music Festivals",
                        Email = "info@debrecen-music.hu",
                        Password = "HashedPassword456!",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Organizer
                    {
                        Id = "org789",
                        Name = "Szeged Expo Management",
                        Email = "hello@szeged-expo.hu",
                        Password = "HashedPassword789!",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    }
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
                            OrganizerId = "org-001",
                            Slug = "teszt-esemeny-1",
                            Name = "Teszt Esemény 1",
                            Description = "Ez egy teszt esemény leírása.",
                            Status = "Published",
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        },
                        new Event
                        {
                            Id = "event-02",
                            OrganizerId = "org-001",
                            Slug = "teszt-esemeny-2",
                            Name = "Teszt Esemény 2",
                            Description = "Második teszt esemény.",
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
                        Name = "Kulturális Fesztivál",
                        City = "Budapest",
                        PostalCode = "1011",
                        AddressLine = "Fő utca 1.",
                        OrganizerId = "org-id-01"
                    },
                    new Venue
                    {
                        Id = "ven-id-02",
                        Name = "Zenei Koncert",
                        City = "Debrecen",
                        PostalCode = "4025",
                        AddressLine = "Kossuth Lajos utca 2.",
                        OrganizerId = "org-id-01"
                    },
                    new Venue
                    {
                        Id = "ven-id-03",
                        Name = "Gasztro Expo",
                        City = "Szeged",
                        PostalCode = "6720",
                        AddressLine = "Klauzál tér 3.",
                        OrganizerId = "org-id-01"
                    },
                    new Venue
                    {
                        Id = "ven-id-05",
                        Name = "Random Venue",
                        City = "Szeged",
                        PostalCode = "6720",
                        AddressLine = "Klauzál tér 10.",
                        OrganizerId = "org-id-01"
                    },
                    new Venue
                    {
                        Id = "ven-id-04",
                        Name = "Gasztro Expo 4",
                        City = "Szeged",
                        PostalCode = "6724",
                        AddressLine = "Klauzál tér 4.",
                        OrganizerId = "org-id-02"
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
                        VenueId = "ven-id-04",
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
            if (!ctx.Organizers.Any())
            {
                var organizers = new List<Organizer>
                {
                    new Organizer
                    {
                        Id = "org-id-01",
                        Name = "Budapest Event Organizers",
                        Email = "contact@budapest-events.hu",
                        Password = "HashedPassword123!",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Organizer
                    {
                        Id = "org-id-02",
                        Name = "Debrecen Music Festivals",
                        Email = "info@debrecen-music.hu",
                        Password = "HashedPassword456!",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new Organizer
                    {
                        Id = "org-id-03",
                        Name = "Szeged Expo Management",
                        Email = "hello@szeged-expo.hu",
                        Password = "HashedPassword789!",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                };

                ctx.Organizers.AddRange(organizers);
                ctx.SaveChanges();
            }
            
            if (!ctx.Seats.Any())
            {
                var seats = new List<Seat>();
                
                // VIP szekciós ülések (sector-id-01) - matriz-id-01
                for (int row = 1; row <= 2; row++)
                {
                    for (int col = 1; col <= 5; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-01",
                            Row = row,
                            Column = col,
                            SeatLabel = $"VIP-{row}{col}",
                            SectorId = "sector-id-01",
                            SeatType = SeatType.Seat,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                // Standard szekciós ülések (sector-id-02) - matriz-id-01
                for (int row = 3; row <= 5; row++)
                {
                    for (int col = 1; col <= 10; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-01",
                            Row = row,
                            Column = col,
                            SeatLabel = $"STD-{row}{col}",
                            SectorId = "sector-id-02",
                            SeatType = col % 5 == 0 ? SeatType.AccessibleSeat : SeatType.Seat,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                // Balcony szekciós ülések (sector-id-03) - matriz-id-01
                for (int row = 6; row <= 7; row++)
                {
                    for (int col = 1; col <= 8; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-01",
                            Row = row,
                            Column = col,
                            SeatLabel = $"BAL-{row}{col}",
                            SectorId = "sector-id-03",
                            SeatType = SeatType.Seat,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                // Folyosó ülések (no sector) - matriz-id-01
                for (int row = 3; row <= 5; row++)
                {
                    for (int col = 6; col <= 6; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-01",
                            Row = row,
                            Column = col,
                            SeatLabel = $"AISLE-{row}",
                            SectorId = null,
                            SeatType = SeatType.Aisle,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                // Front szekciós ülések (sector-id-04) - matriz-id-02
                for (int row = 1; row <= 3; row++)
                {
                    for (int col = 1; col <= 8; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-02",
                            Row = row,
                            Column = col,
                            SeatLabel = $"FRONT-{row}{col}",
                            SectorId = "sector-id-04",
                            SeatType = SeatType.Seat,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                // Back szekciós ülések (sector-id-05) - matriz-id-02
                for (int row = 4; row <= 6; row++)
                {
                    for (int col = 1; col <= 8; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-02",
                            Row = row,
                            Column = col,
                            SeatLabel = $"BACK-{row}{col}",
                            SectorId = "sector-id-05",
                            SeatType = SeatType.Seat,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                // Premium szekciós ülések (sector-id-06) - matriz-id-03
                for (int row = 1; row <= 4; row++)
                {
                    for (int col = 1; col <= 6; col++)
                    {
                        seats.Add(new Seat
                        {
                            Id = Guid.NewGuid().ToString(),
                            MatrixId = "matrix-id-03",
                            Row = row,
                            Column = col,
                            SeatLabel = $"PREM-{row}{col}",
                            SectorId = "sector-id-06",
                            SeatType = SeatType.Seat,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow
                        });
                    }
                }
                
                ctx.Seats.AddRange(seats);
                ctx.SaveChanges();
            }

            // 7. EventOccurrences
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
                        StartsAtUtc = DateTime.UtcNow.AddDays(7),
                        EndsAtUtc = DateTime.UtcNow.AddDays(7).AddHours(2),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(7),
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new EventOccurrence
                    {
                        Id = "occ-02",
                        EventId = "event-01",
                        VenueId = "ven-id-01",
                        AuditoriumId = "aud-id-01",
                        StartsAtUtc = DateTime.UtcNow.AddDays(6),
                        EndsAtUtc = DateTime.UtcNow.AddDays(6).AddHours(1),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(9),
                        Status = "Published",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    },
                    new EventOccurrence
                    {
                        Id = "occ-03",
                        EventId = "event-02",
                        VenueId = "ven-id-04",
                        AuditoriumId = "aud-id-03",
                        StartsAtUtc = DateTime.UtcNow.AddDays(10),
                        EndsAtUtc = DateTime.UtcNow.AddDays(10).AddHours(3),
                        BookingOpenAtUtc = DateTime.UtcNow,
                        BookingCloseAtUtc = DateTime.UtcNow.AddDays(10),
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
