using Data;
using Entities.Dtos.Organizer;
using Entities.Models;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class OrganizerServiceTests
    {
        private AppDbContext _dbContext;
        private OrganizerService _organizerService;
        private PasswordHasher<Organizer> _passwordHasher;

        [SetUp]
        public void SetUp()
        {
            // Use a unique database name for each test to ensure test isolation
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _organizerService = new OrganizerService(_dbContext);
            _passwordHasher = new PasswordHasher<Organizer>();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task CreateAsync_ValidInput_CreatesOrganizerAndHashesPassword()
        {
            // Arrange
            var dto = new OrganizerCreateDto
            {
                Email = "test@organizer.com",
                Name = "Test Organizer",
                Password = "SecurePassword123"
            };

            // Act
            var result = await _organizerService.CreateAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Email, result.Email);
            Assert.AreEqual(dto.Name, result.Name);

            // Verify database state
            var savedOrganizer = await _dbContext.Organizers.FirstOrDefaultAsync(o => o.Id == result.Id);
            Assert.IsNotNull(savedOrganizer);

            // Ensure password was hashed, not saved in plain text
            Assert.AreNotEqual("SecurePassword123", savedOrganizer.PasswordHash);

            // Verify hash is valid
            var verificationResult = _passwordHasher.VerifyHashedPassword(savedOrganizer, savedOrganizer.PasswordHash, "SecurePassword123");
            Assert.AreEqual(PasswordVerificationResult.Success, verificationResult);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllOrganizersWithVenues()
        {
            // Arrange
            var org1 = new Organizer { Email = "org1@test.com", Name = "Org 1" };
            org1.Venues.Add(new Venue { Id = "V1", Name = "Venue 1", City = "City", PostalCode = "1234", AddressLine = "Line" });

            var org2 = new Organizer { Email = "org2@test.com", Name = "Org 2" };

            _dbContext.Organizers.AddRange(org1, org2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _organizerService.GetAllAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.First(o => o.Id == org1.Id).Venues.Count);
            Assert.AreEqual(0, result.First(o => o.Id == org2.Id).Venues.Count);
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsOrganizer()
        {
            // Arrange
            var org = new Organizer { Email = "findme@test.com", Name = "Find Me" };
            _dbContext.Organizers.Add(org);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _organizerService.GetByIdAsync(org.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(org.Id, result.Id);
            Assert.AreEqual("findme@test.com", result.Email);
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _organizerService.GetByIdAsync("invalid-id");

            // Assert
            Assert.IsNull(result);
        }

        [TestCase("HELLO@TEST.COM")]
        [TestCase("hello@test.com")]
        [TestCase(" Hello@Test.com ")]
        public async Task GetByEmailAsync_ExistingEmail_IsCaseInsensitive_ReturnsOrganizer(string searchEmail)
        {
            // Arrange
            var org = new Organizer { Email = "hello@test.com", Name = "Hello Org" };
            _dbContext.Organizers.Add(org);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _organizerService.GetByEmailAsync(searchEmail);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(org.Id, result.Id);
        }

        [Test]
        public async Task UpdateAsync_ExistingOrganizer_UpdatesName()
        {
            // Arrange
            var org = new Organizer { Email = "update@test.com", Name = "Old Name" };
            _dbContext.Organizers.Add(org);
            await _dbContext.SaveChangesAsync();

            var updateDto = new OrganizerUpdateDto { Name = "New Name" };

            // Act
            var result = await _organizerService.UpdateAsync(org.Id, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Name", result.Name);

            var dbEntity = await _dbContext.Organizers.FindAsync(org.Id);
            Assert.AreEqual("New Name", dbEntity.Name);
        }

        [Test]
        public void UpdateAsync_NonExistingOrganizer_ThrowsArgumentException()
        {
            // Arrange
            var updateDto = new OrganizerUpdateDto { Name = "New Name" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _organizerService.UpdateAsync("invalid-id", updateDto));
            Assert.AreEqual("Organizer not found.", ex.Message);
        }

        [Test]
        public async Task UpdateProfileAsync_ExistingOrganizer_UpdatesProfileAndIncludesVenues()
        {
            // Arrange
            var org = new Organizer { Email = "profile@test.com", Name = "Old Profile" };
            org.Venues.Add(new Venue { Id = "V1", Name = "Venue 1", City = "C", PostalCode = "P", AddressLine = "A" });
            _dbContext.Organizers.Add(org);
            await _dbContext.SaveChangesAsync();

            var updateDto = new OrganizerProfileUpdateDto { Name = "Updated Profile" };

            // Act
            var result = await _organizerService.UpdateProfileAsync(org.Id, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Profile", result.Name);
            Assert.AreEqual(1, result.Venues.Count);
        }

        [Test]
        public async Task ChangePasswordAsync_ValidCurrentPassword_ChangesPasswordHash()
        {
            // Arrange
            var org = new Organizer { Email = "pass@test.com", Name = "Pass Org" };
            org.PasswordHash = _passwordHasher.HashPassword(org, "OldPassword123");
            _dbContext.Organizers.Add(org);
            await _dbContext.SaveChangesAsync();

            var dto = new OrganizerPasswordChangeDto
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewSecurePassword456"
            };

            // Act
            await _organizerService.ChangePasswordAsync(org.Id, dto);

            // Assert
            var dbEntity = await _dbContext.Organizers.FindAsync(org.Id);
            var verifyResult = _passwordHasher.VerifyHashedPassword(dbEntity, dbEntity.PasswordHash, "NewSecurePassword456");
            Assert.AreEqual(PasswordVerificationResult.Success, verifyResult);
        }

        [Test]
        public void ChangePasswordAsync_InvalidCurrentPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var org = new Organizer { Email = "pass@test.com", Name = "Pass Org" };
            org.PasswordHash = _passwordHasher.HashPassword(org, "OldPassword123");
            _dbContext.Organizers.Add(org);
            _dbContext.SaveChanges();

            var dto = new OrganizerPasswordChangeDto
            {
                CurrentPassword = "WrongPassword",
                NewPassword = "NewSecurePassword456"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _organizerService.ChangePasswordAsync(org.Id, dto));
            Assert.AreEqual("Current password is incorrect.", ex.Message);
        }

        [Test]
        public async Task DeleteAsync_ExistingId_DeletesAndReturnsTrue()
        {
            // Arrange
            var org = new Organizer { Email = "delete@test.com", Name = "Delete Org" };
            _dbContext.Organizers.Add(org);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _organizerService.DeleteAsync(org.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, await _dbContext.Organizers.CountAsync());
        }

        [Test]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _organizerService.DeleteAsync("invalid-id");

            // Assert
            Assert.IsFalse(result);
        }
    }
}