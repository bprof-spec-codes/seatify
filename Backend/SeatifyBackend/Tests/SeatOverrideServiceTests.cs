using Data;
using Entities.Dtos.SeatOverride;
using Entities.Models;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class SeatOverrideServiceTests
    {
        private AppDbContext _context;
        private SeatOverrideService _seatOverrideService;

        [SetUp]
        public void Setup()
        {
            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

            _context = new AppDbContext(options);
            _seatOverrideService = new SeatOverrideService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        // --------------------------------------------------------------------------
        // GetEffectiveSeatMapForEventAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public async Task GetEffectiveSeatMapForEventAsync_NotExistingMatrixId_ShouldReturnNull()
        {
            // Arrange
            string invalidMatrixId = "InvalidMatrix";
            string validEventId = "Event1";

            // Act
            var result = await _seatOverrideService.GetEffectiveSeatMapForEventAsync(validEventId, invalidMatrixId, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEffectiveSeatMapForEventAsync_NotExistingEvent_ShouldReturnNull()
        {
            // Arrange
            LayoutMatrix layoutMatrix = new LayoutMatrix
            {
                Id = "Matrix_Standard_10x10",
                AuditoriumId = "Aud_Main_001",
                Name = "Main Floor Layout",
                Rows = 10,
                Columns = 10,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _context.LayoutMatrices.Add(layoutMatrix);
            await _context.SaveChangesAsync();

            // Act
            var result = await _seatOverrideService.GetEffectiveSeatMapForEventAsync("NonExistentEvent", "Matrix_Standard_10x10", CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEffectiveSeatMapForEventAsync_ValidInput_ReturnsEffectiveSeatMapDto()
        {
            // Arrange
            LayoutMatrix layoutMatrix = new LayoutMatrix
            {
                Id = "Matrix_Standard_10x10",
                AuditoriumId = "Aud_Main_001",
                Name = "Main Floor Layout",
                Rows = 10,
                Columns = 10
            };
            _context.LayoutMatrices.Add(layoutMatrix);

            Seat seat = new Seat
            {
                Id = "Seat_R1_C1",
                MatrixId = "Matrix_Standard_10x10",
                Row = 1,
                Column = 1,
                SeatLabel = "A-1",
                SectorId = "Sector_VIP_001",
                PriceOverride = 7500.00m,
                SeatType = SeatType.Seat
            };
            _context.Seats.Add(seat);

            Event mockEvent = new Event
            {
                Id = "Event_Alpha_2026",
                OrganizerId = "Org_Test_123",
                Name = "Rock Friday Night"
            };
            _context.Events.Add(mockEvent);

            Sector sector = new Sector
            {
                Id = "Sector_VIP_001",
                AuditoriumId = "Aud_Main_001",
                BasePrice = 12500.50m
            };
            _context.Sectors.Add(sector);

            EventSeatOverride eventOverride = new EventSeatOverride
            {
                Id = "Override_Event_Alpha_Seat_1",
                EventId = "Event_Alpha_2026",
                SeatId = "Seat_R1_C1",
                SectorId = "Sector_VIP_001",
                SeatType = SeatType.Seat,
                PriceOverride = 15000.00m // Custom price for this event
            };
            _context.EventSeatOverrides.Add(eventOverride);

            await _context.SaveChangesAsync();

            // Act
            var result = await _seatOverrideService.GetEffectiveSeatMapForEventAsync("Event_Alpha_2026", "Matrix_Standard_10x10", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Matrix_Standard_10x10", result.MatrixId);
            Assert.AreEqual("event", result.Context);
            Assert.AreEqual(1, result.Seats.Count);

            var effectiveSeat = result.Seats.First();
            Assert.AreEqual("Seat_R1_C1", effectiveSeat.SeatId);
            Assert.AreEqual(15000.00m, effectiveSeat.FinalPrice); // Taken from event override
            Assert.AreEqual("event", effectiveSeat.PriceSource);
        }

        // --------------------------------------------------------------------------
        // GetEffectiveSeatMapForOccurrenceAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public async Task GetEffectiveSeatMapForOccurrenceAsync_NotExistingOccurrence_ShouldReturnNull()
        {
            // Arrange
            LayoutMatrix layoutMatrix = new LayoutMatrix { Id = "Mat1", AuditoriumId = "Aud1" };
            _context.LayoutMatrices.Add(layoutMatrix);
            await _context.SaveChangesAsync();

            // Act
            var result = await _seatOverrideService.GetEffectiveSeatMapForOccurrenceAsync("InvalidOccurrence", "Mat1", CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEffectiveSeatMapForOccurrenceAsync_ValidInput_OccurrenceOverrideWins()
        {
            // Arrange
            _context.LayoutMatrices.Add(new LayoutMatrix { Id = "Mat1", AuditoriumId = "Aud1" });
            _context.Sectors.Add(new Sector { Id = "Sec1", AuditoriumId = "Aud1", BasePrice = 1000m });
            _context.Seats.Add(new Seat { Id = "Seat1", MatrixId = "Mat1", SectorId = "Sec1", PriceOverride = 2000m });

            _context.Events.Add(new Event { Id = "Ev1" });
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ1", EventId = "Ev1" });

            // Event override sets price to 3000
            _context.EventSeatOverrides.Add(new EventSeatOverride { EventId = "Ev1", SeatId = "Seat1", PriceOverride = 3000m });

            // Occurrence override sets price to 5000 (Should be the highest priority)
            _context.OccurrenceSeatOverrides.Add(new OccurrenceSeatOverride { OccurrenceId = "Occ1", SeatId = "Seat1", PriceOverride = 5000m });

            await _context.SaveChangesAsync();

            // Act
            var result = await _seatOverrideService.GetEffectiveSeatMapForOccurrenceAsync("Occ1", "Mat1", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("occurrence", result.Context);
            Assert.AreEqual(1, result.Seats.Count);

            var effectiveSeat = result.Seats.First();
            Assert.AreEqual(5000m, effectiveSeat.FinalPrice);
            Assert.AreEqual("occurrence", effectiveSeat.PriceSource);
        }

        // --------------------------------------------------------------------------
        // BulkUpsertEventOverrideAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public void BulkUpsertEventOverrideAsync_NullOrEmptySeatIds_ThrowsArgumentException()
        {
            // Arrange
            BulkSeatOverrideDto dto = new BulkSeatOverrideDto { SeatIds = new List<string>() };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _seatOverrideService.BulkUpsertEventOverrideAsync("Ev1", dto, CancellationToken.None));
            Assert.AreEqual("At least one SeatId must be provided.", ex.Message);
        }

        [Test]
        public void BulkUpsertEventOverrideAsync_EventNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            BulkSeatOverrideDto dto = new BulkSeatOverrideDto { SeatIds = new List<string> { "Seat1" } };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _seatOverrideService.BulkUpsertEventOverrideAsync("MissingEvent", dto, CancellationToken.None));
            Assert.IsTrue(ex.Message.Contains("Event not found"));
        }

        [Test]
        public async Task BulkUpsertEventOverrideAsync_ValidInput_InsertsAndUpdatesCorrectly()
        {
            // Arrange
            _context.Events.Add(new Event { Id = "Ev1" });
            _context.Seats.Add(new Seat { Id = "Seat1", MatrixId = "Mat1" });
            _context.Seats.Add(new Seat { Id = "Seat2", MatrixId = "Mat1" });

            // Existing override for Seat1
            _context.EventSeatOverrides.Add(new EventSeatOverride { EventId = "Ev1", SeatId = "Seat1", PriceOverride = 100m });
            await _context.SaveChangesAsync();

            BulkSeatOverrideDto dto = new BulkSeatOverrideDto
            {
                SeatIds = new List<string> { "Seat1", "Seat2" },
                PriceOverride = 500m
            };

            // Act
            var response = await _seatOverrideService.BulkUpsertEventOverrideAsync("Ev1", dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.UpsertedCount);
            Assert.Contains("Seat1", response.AffectedSeatIds);
            Assert.Contains("Seat2", response.AffectedSeatIds);

            var dbOverrides = await _context.EventSeatOverrides.Where(e => e.EventId == "Ev1").ToListAsync();
            Assert.AreEqual(2, dbOverrides.Count);
            Assert.IsTrue(dbOverrides.All(o => o.PriceOverride == 500m));
        }

        // --------------------------------------------------------------------------
        // BulkUpsertOccurrenceOverrideAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public void BulkUpsertOccurrenceOverrideAsync_OccurrenceNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            BulkSeatOverrideDto dto = new BulkSeatOverrideDto { SeatIds = new List<string> { "Seat1" } };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _seatOverrideService.BulkUpsertOccurrenceOverrideAsync("MissingOcc", dto, CancellationToken.None));
            Assert.IsTrue(ex.Message.Contains("Occurrence not found"));
        }

        [Test]
        public async Task BulkUpsertOccurrenceOverrideAsync_ValidInput_UpdatesExistingAndAddsNew()
        {
            // Arrange
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ1", EventId = "Ev1" });
            _context.Seats.Add(new Seat { Id = "S1", MatrixId = "M1" });

            _context.OccurrenceSeatOverrides.Add(new OccurrenceSeatOverride
            {
                OccurrenceId = "Occ1",
                SeatId = "S1",
                SeatType = SeatType.Seat
            });
            await _context.SaveChangesAsync();

            BulkSeatOverrideDto dto = new BulkSeatOverrideDto
            {
                SeatIds = new List<string> { "S1" },
                SeatType = "AccessibleSeat"
            };

            // Act
            var response = await _seatOverrideService.BulkUpsertOccurrenceOverrideAsync("Occ1", dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.UpsertedCount);

            var updatedOverride = await _context.OccurrenceSeatOverrides.FirstAsync();
            Assert.AreEqual(SeatType.AccessibleSeat, updatedOverride.SeatType);
        }

        [Test]
        public async Task BulkUpsertOccurrenceOverrideAsync_ClearSectorAndPrice_RemovesValues()
        {
            // Arrange
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ1", EventId = "Ev1" });
            _context.Sectors.Add(new Sector { Id = "Sec1", AuditoriumId = "Aud1" });
            _context.Seats.Add(new Seat { Id = "S1", MatrixId = "M1" });

            _context.OccurrenceSeatOverrides.Add(new OccurrenceSeatOverride
            {
                OccurrenceId = "Occ1",
                SeatId = "S1",
                SectorId = "Sec1",
                PriceOverride = 999m
            });
            await _context.SaveChangesAsync();

            BulkSeatOverrideDto dto = new BulkSeatOverrideDto
            {
                SeatIds = new List<string> { "S1" },
                ClearSector = true,
                ClearPriceOverride = true
            };

            // Act
            await _seatOverrideService.BulkUpsertOccurrenceOverrideAsync("Occ1", dto, CancellationToken.None);

            // Assert
            var updatedOverride = await _context.OccurrenceSeatOverrides.FirstAsync();
            Assert.IsNull(updatedOverride.SectorId);
            Assert.IsNull(updatedOverride.PriceOverride);
        }
    }
}