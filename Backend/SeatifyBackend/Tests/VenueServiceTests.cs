using Data;
using Entities.Dtos.Venue;
using Entities.Models;
using Logic.Helper;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class VenueServiceTests
    {
        private AppDbContext _context;
        private VenueService _venueService;

        [SetUp]
        public void Setup()
        {
            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

            _context = new AppDbContext(options);
            DtoProvider dtoProvider = new DtoProvider();
            _venueService = new VenueService(_context, dtoProvider);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        // --------------------------------------------------------------------------
        // CreateVenueAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public void CreateVenueAsync_InvalidOrganizerId_ThrowsException()
        {
            // Arrange
            VenueCreateDto venueCreateDto = new VenueCreateDto();
            string organizerId = "bad_id";

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _venueService.CreateVenueAsync(venueCreateDto, organizerId));
            Assert.AreEqual("Organizer does not exist.", ex.Message);
        }

        [Test]
        public async Task CreateVenueAsync_ValidOrganizerId_ReturnVenueObject()
        {
            // Arrange
            Organizer organizer = new Organizer();
            organizer.Id = "Org_Test_123";
            organizer.Name = "Test Organizer Ltd.";
            organizer.Email = "info@testorganizer.com";
            organizer.PasswordHash = "hashed_password_sample";
            organizer.CreatedAtUtc = DateTime.UtcNow;
            organizer.UpdatedAtUtc = DateTime.UtcNow;

            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            VenueCreateDto venueCreateDto = new VenueCreateDto();
            venueCreateDto.Name = "Grand Hall";
            venueCreateDto.City = "Budapest";
            venueCreateDto.PostalCode = "1051";
            venueCreateDto.AddressLine = "Erzsébet tér 1.";

            // Act
            Venue venue = await _venueService.CreateVenueAsync(venueCreateDto, "Org_Test_123");

            // Assert
            Assert.IsNotNull(venue);
            Assert.IsNotNull(venue.Id);
            Assert.IsTrue(venue.Id.Length > 0);
            Assert.AreEqual("Grand Hall", venue.Name);
            Assert.AreEqual("Budapest", venue.City);
            Assert.AreEqual("1051", venue.PostalCode);
            Assert.AreEqual("Erzsébet tér 1.", venue.AddressLine);
            Assert.AreEqual("Org_Test_123", venue.OrganizerId);
        }

        // --------------------------------------------------------------------------
        // GetVenueByIdAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public void GetVenueByIdAsync_VenueDoesNotExist_ThrowsException()
        {
            // Arrange
            // Empty database

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _venueService.GetVenueByIdAsync("NonExistentId"));
            Assert.AreEqual("Venue does not exist!", ex.Message);
        }

        [Test]
        public async Task GetVenueByIdAsync_ValidId_ReturnsVenueViewDto()
        {
            // Arrange
            Venue venue = new Venue();
            venue.Id = "Venue_1";
            venue.OrganizerId = "Org_1";
            venue.Name = "Main Arena";
            venue.City = "Debrecen";

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            // Act
            var result = await _venueService.GetVenueByIdAsync("Venue_1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Main Arena", result.Name);
            Assert.AreEqual("Debrecen", result.City);
        }

        // --------------------------------------------------------------------------
        // GetAllVenuesAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public async Task GetAllVenuesAsync_ReturnsListOfVenues()
        {
            // Arrange
            Venue venue1 = new Venue();
            venue1.Id = "Venue_1";
            venue1.Name = "Arena 1";

            Venue venue2 = new Venue();
            venue2.Id = "Venue_2";
            venue2.Name = "Arena 2";

            _context.Venues.Add(venue1);
            _context.Venues.Add(venue2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _venueService.GetAllVenuesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        // --------------------------------------------------------------------------
        // UpdateVenueByIdAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public void UpdateVenueByIdAsync_VenueDoesNotExist_ThrowsException()
        {
            // Arrange
            VenueUpdateDto updateDto = new VenueUpdateDto();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _venueService.UpdateVenueByIdAsync(updateDto, "InvalidVenueId", "Org_1"));
            Assert.AreEqual("Venue does not exist!", ex.Message);
        }

        [Test]
        public async Task UpdateVenueByIdAsync_NotOwnedByOrganizer_ThrowsException()
        {
            // Arrange
            Venue venue = new Venue();
            venue.Id = "Venue_1";
            venue.OrganizerId = "Org_Owner";
            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            VenueUpdateDto updateDto = new VenueUpdateDto();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _venueService.UpdateVenueByIdAsync(updateDto, "Venue_1", "Org_Hacker"));
            Assert.AreEqual("The venue does not belong to the logged-in user!", ex.Message);
        }

        [Test]
        public async Task UpdateVenueByIdAsync_ValidInput_UpdatesAndReturnsVenue()
        {
            // Arrange
            Venue venue = new Venue();
            venue.Id = "Venue_1";
            venue.OrganizerId = "Org_Owner";
            venue.Name = "Old Name";
            venue.City = "Old City";

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            VenueUpdateDto updateDto = new VenueUpdateDto();
            updateDto.Id = "Venue_1";
            updateDto.Name = "New Name";
            updateDto.City = "New City";
            updateDto.PostalCode = "1051";
            updateDto.AddressLine = "Erzsébet tér 1.";

            // Act
            var result = await _venueService.UpdateVenueByIdAsync(updateDto, "Venue_1", "Org_Owner");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Name", result.Name);
            Assert.AreEqual("New City", result.City);

            var dbVenue = await _context.Venues.FindAsync("Venue_1");
            Assert.AreEqual("New Name", dbVenue.Name);
            Assert.AreEqual("New City", dbVenue.City);
        }

        // --------------------------------------------------------------------------
        // DeleteVenueByIdAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public void DeleteVenueByIdAsync_VenueDoesNotExist_ThrowsException()
        {
            // Arrange
            // Empty database

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _venueService.DeleteVenueByIdAsync("Org_1", "InvalidVenueId"));
            Assert.AreEqual("Venue does not exist!", ex.Message);
        }

        [Test]
        public async Task DeleteVenueByIdAsync_NotOwnedByOrganizer_ThrowsException()
        {
            // Arrange
            Venue venue = new Venue();
            venue.Id = "Venue_1";
            venue.OrganizerId = "Org_Owner";

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _venueService.DeleteVenueByIdAsync("Org_Hacker", "Venue_1"));
            Assert.AreEqual("The venue does not belong to the logged-in user!", ex.Message);
        }

        [Test]
        public async Task DeleteVenueByIdAsync_ValidInput_DeletesAndReturnsTrue()
        {
            // Arrange
            Venue venue = new Venue();
            venue.Id = "Venue_1";
            venue.OrganizerId = "Org_Owner";

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            // Act
            bool result = await _venueService.DeleteVenueByIdAsync("Org_Owner", "Venue_1");

            // Assert
            Assert.IsTrue(result);
            var dbVenue = await _context.Venues.FindAsync("Venue_1");
            Assert.IsNull(dbVenue); // Ensure it was removed from the database
        }

        // --------------------------------------------------------------------------
        // GetVenuesByOrganizerIdAsync Tests
        // --------------------------------------------------------------------------

        [Test]
        public async Task GetVenuesByOrganizerIdAsync_ReturnsFilteredList()
        {
            // Arrange
            string targetOrganizerId = "Target_Org";

            Venue venue1 = new Venue();
            venue1.Id = "Venue_1";
            venue1.OrganizerId = targetOrganizerId;

            Venue venue2 = new Venue();
            venue2.Id = "Venue_2";
            venue2.OrganizerId = targetOrganizerId;

            Venue venue3 = new Venue();
            venue3.Id = "Venue_3";
            venue3.OrganizerId = "Other_Org"; // Belongs to someone else

            _context.Venues.Add(venue1);
            _context.Venues.Add(venue2);
            _context.Venues.Add(venue3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _venueService.GetVenuesByOrganizerIdAsync(targetOrganizerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); // Should only return the two venues belonging to Target_Org
            Assert.IsTrue(result.All(v => v.Id == "Venue_1" || v.Id == "Venue_2"));
        }
    }
}