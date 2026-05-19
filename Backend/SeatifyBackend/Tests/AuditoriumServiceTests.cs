using Data;
using Entities.Dtos.Auditorium;
using Entities.Models;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class AuditoriumServiceTests
    {
        private AppDbContext _context;
        private AuditoriumService _auditoriumService;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);
            _auditoriumService = new AuditoriumService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ValidInput_CreatesAuditorium()
        {
            // Arrange
            var venueId = "Venue_1";
            _context.Venues.Add(new Venue { Id = venueId, Name = "Test Venue" });
            await _context.SaveChangesAsync();

            var dto = new AuditoriumCreateDto { Name = "Main Hall", Currency = "USD", Description = "Test" };

            // Act
            var result = await _auditoriumService.CreateAsync(venueId, dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Main Hall", result.Name);
            Assert.AreEqual("USD", result.Currency);
            
            var dbEntity = await _context.Auditoriums.FirstOrDefaultAsync(a => a.Id == result.Id);
            Assert.IsNotNull(dbEntity);
            Assert.AreEqual("Main Hall", dbEntity.Name);
        }

        [Test]
        public void CreateAsync_MissingName_ThrowsArgumentException()
        {
            var dto = new AuditoriumCreateDto { Name = "" };
            Assert.ThrowsAsync<ArgumentException>(() => _auditoriumService.CreateAsync("Venue_1", dto, CancellationToken.None));
        }

        [Test]
        public void CreateAsync_NonExistingVenue_ThrowsArgumentException()
        {
            var dto = new AuditoriumCreateDto { Name = "Main Hall" };
            Assert.ThrowsAsync<ArgumentException>(() => _auditoriumService.CreateAsync("NonExistent", dto, CancellationToken.None));
        }

        [Test]
        public async Task UpdateAsync_ValidInput_UpdatesAuditorium()
        {
            // Arrange
            var audId = "Aud_1";
            _context.Auditoriums.Add(new Auditorium { Id = audId, Name = "Old Name", VenueId = "Venue_1", Currency = "EUR" });
            await _context.SaveChangesAsync();

            var dto = new AuditoriumCreateDto { Name = "New Name", Currency = "GBP", Description = "New Desc" };

            // Act
            var result = await _auditoriumService.UpdateAsync(audId, dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Name", result.Name);
            Assert.AreEqual("GBP", result.Currency);
            
            var dbEntity = await _context.Auditoriums.FirstOrDefaultAsync(a => a.Id == audId);
            Assert.AreEqual("New Name", dbEntity.Name);
            Assert.AreEqual("GBP", dbEntity.Currency);
        }

        [Test]
        public async Task DeleteAsync_ExistingAuditorium_ReturnsTrueAndDeletes()
        {
            // Arrange
            var audId = "Aud_1";
            _context.Auditoriums.Add(new Auditorium { Id = audId, Name = "To Be Deleted", VenueId = "V1" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _auditoriumService.DeleteAsync(audId, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            var dbEntity = await _context.Auditoriums.FirstOrDefaultAsync(a => a.Id == audId);
            Assert.IsNull(dbEntity);
        }

        [Test]
        public async Task HasBookingsAsync_WithConfirmedBookings_ReturnsTrue()
        {
            // Arrange
            var audId = "Aud_1";
            var occId = "Occ_1";
            
            _context.EventOccurrences.Add(new EventOccurrence { Id = occId, AuditoriumId = audId });
            _context.Reservations.Add(new Reservation { Id = "Res_1", EventOccurrenceId = occId, Status = "Confirmed" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _auditoriumService.HasBookingsAsync(audId, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsDtoWithMatrices()
        {
            // Arrange
            var audId = "Aud_1";
            _context.Auditoriums.Add(new Auditorium { Id = audId, Name = "Hall A", VenueId = "V1" });
            _context.LayoutMatrices.Add(new LayoutMatrix { Id = "LM_1", AuditoriumId = audId, Name = "Matrix 1", Rows = 10, Columns = 10 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _auditoriumService.GetByIdAsync(audId, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Hall A", result.Name);
            Assert.AreEqual(1, result.LayoutMatrices.Count);
            Assert.AreEqual("Matrix 1", result.LayoutMatrices.First().Name);
        }
    }
}
