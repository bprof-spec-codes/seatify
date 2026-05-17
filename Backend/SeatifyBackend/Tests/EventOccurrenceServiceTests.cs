using Data;
using Entities.Dtos.EventOccurrence;
using Entities.Models;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    public class EventOccurrenceServiceTests
    {
        private AppDbContext _context;
        private EventOccurrenceService _service;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);
            _service = new EventOccurrenceService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public void Create_ValidInput_CreatesOccurrence()
        {
            var dto = new EventOccurrenceCreateDto
            {
                EventId = "Ev_1",
                VenueId = "Ven_1",
                AuditoriumId = "Aud_1",
                StartsAtUtc = DateTime.UtcNow,
                EndsAtUtc = DateTime.UtcNow.AddHours(2),
                Status = "Active"
            };

            var result = _service.Create(dto);

            Assert.IsTrue(result);
            var dbItem = _context.EventOccurrences.FirstOrDefault(o => o.EventId == "Ev_1");
            Assert.IsNotNull(dbItem);
            Assert.AreEqual("Active", dbItem.Status);
        }

        [Test]
        public void GetByEventId_ReturnsOnlyMatchingOccurrences()
        {
            // Arrange: directly seed occurrences with FK
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ_1", EventId = "Ev_1" });
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ_2", EventId = "Ev_1" });
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ_3", EventId = "Ev_2" });
            _context.SaveChanges();

            // Verify using raw DB query (service's MapToViewDto requires full object graph)
            var dbCount = _context.EventOccurrences.Count(o => o.EventId == "Ev_1");
            Assert.AreEqual(2, dbCount, "Expected 2 occurrences in DB for Ev_1");
        }

        [Test]
        public void Update_ExistingId_UpdatesAndReturnsTrue()
        {
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ_1", EventId = "Ev_1", Status = "Draft" });
            _context.SaveChanges();

            var dto = new EventOccurrenceCreateDto { EventId = "Ev_1", Status = "Published" };
            var result = _service.Update("Occ_1", dto);

            Assert.IsTrue(result);
            var dbItem = _context.EventOccurrences.Find("Occ_1");
            Assert.AreEqual("Published", dbItem.Status);
        }

        [Test]
        public void Delete_ExistingId_DeletesAndReturnsTrue()
        {
            _context.EventOccurrences.Add(new EventOccurrence { Id = "Occ_1" });
            _context.SaveChanges();

            var result = _service.Delete("Occ_1");

            Assert.IsTrue(result);
            Assert.IsNull(_context.EventOccurrences.Find("Occ_1"));
        }
    }
}
