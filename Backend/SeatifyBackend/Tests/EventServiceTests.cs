using Data;
using Entities.Dtos.Event;
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
    public class EventServiceTests
    {
        private AppDbContext _context;
        private EventService _eventService;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);
            _eventService = new EventService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ValidInput_CreatesEvent()
        {
            // Arrange
            _context.Organizers.Add(new Organizer { Id = "Org_1", Email = "test@test.com", Name = "Org" });
            await _context.SaveChangesAsync();

            var dto = new EventCreateDto
            {
                OrganizerId = "Org_1",
                Name = "New Event",
                Slug = "new-event",
                Status = "Draft",
                Currency = "USD"
            };

            // Act
            var result = await _eventService.CreateAsync(dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Event", result.Name);
            Assert.AreEqual("new-event", result.Slug);

            var dbEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == result.Id);
            Assert.IsNotNull(dbEvent);
            Assert.AreEqual("New Event", dbEvent.Name);
        }

        [Test]
        public async Task CreateAsync_DuplicateSlug_ThrowsArgumentException()
        {
            // Arrange
            _context.Organizers.Add(new Organizer { Id = "Org_1", Email = "test@test.com", Name = "Org" });
            _context.Events.Add(new Event { Id = "Ev_1", OrganizerId = "Org_1", Name = "Event 1", Slug = "existing-slug" });
            await _context.SaveChangesAsync();

            var dto = new EventCreateDto
            {
                OrganizerId = "Org_1",
                Name = "Event 2",
                Slug = "existing-slug"
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _eventService.CreateAsync(dto, CancellationToken.None));
        }

        [Test]
        public async Task UpdateAsync_ValidInput_UpdatesEvent()
        {
            // Arrange
            var evId = "Ev_2";
            _context.Events.Add(new Event { Id = evId, OrganizerId = "Org_1", Name = "Old Name", Slug = "old-slug", Status = "Draft", Currency = "EUR" });
            await _context.SaveChangesAsync();

            var dto = new EventUpdateDto
            {
                Name = "New Name",
                Slug = "new-slug",
                Status = "Published",
                Currency = "USD"
            };

            // Act
            var result = await _eventService.UpdateAsync(evId, dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Name", result.Name);
            Assert.AreEqual("Published", result.Status);

            var dbEvent = await _context.Events.FindAsync(evId);
            Assert.AreEqual("New Name", dbEvent.Name);
        }

        [Test]
        public async Task DeleteAsync_ExistingEvent_DeletesEvent()
        {
            // Arrange
            var evId = "Ev_3";
            _context.Events.Add(new Event { Id = evId, OrganizerId = "Org_1", Name = "To Delete", Slug = "to-delete" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.DeleteAsync(evId, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            var dbEvent = await _context.Events.FindAsync(evId);
            Assert.IsNull(dbEvent);
        }

        [Test]
        public async Task GetPublicAsync_ReturnsOnlyPublishedEvents()
        {
            // Arrange
            _context.Events.Add(new Event { Id = "Ev_4", Status = "Published", Name = "Pub1", Slug = "pub1", OrganizerId = "Org_1" });
            _context.Events.Add(new Event { Id = "Ev_5", Status = "Draft", Name = "Draft1", Slug = "draft1", OrganizerId = "Org_1" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventService.GetPublicAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Pub1", result.First().Name);
        }
    }
}
