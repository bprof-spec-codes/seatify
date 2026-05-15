using Data;
using Entities.Dtos.Auth;
using Entities.Models;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class AuthServiceTests
    {
        private AppDbContext _context;
        private Mock<IJwtTokenService> _mockJwtTokenService;
        private AuthService _authService;
        private PasswordHasher<Organizer> _passwordHasher;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _passwordHasher = new PasswordHasher<Organizer>();
            _authService = new AuthService(_context, _mockJwtTokenService.Object);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task RegisterAsync_ValidDto_CreatesOrganizerAndReturnsToken()
        {
            // Arrange
            var dto = new OrganizerRegisterDto
            {
                Email = "test@example.com",
                Name = "Test Organizer",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var expectedToken = "mock_token";
            _mockJwtTokenService.Setup(x => x.GenerateToken(It.IsAny<Organizer>())).Returns(expectedToken);
            _mockJwtTokenService.Setup(x => x.GetExpiryUtc()).Returns(DateTime.UtcNow.AddHours(1));

            // Act
            var result = await _authService.RegisterAsync(dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.AreEqual(expectedToken, result.Token);

            var dbOrganizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Email == "test@example.com");
            Assert.IsNotNull(dbOrganizer);
            Assert.AreEqual("Test Organizer", dbOrganizer.Name);
            Assert.IsNotNull(dbOrganizer.PasswordHash);
        }

        [Test]
        public void RegisterAsync_MismatchedPasswords_ThrowsArgumentException()
        {
            var dto = new OrganizerRegisterDto
            {
                Email = "test@example.com",
                Name = "Test Organizer",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword"
            };

            Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(dto, CancellationToken.None));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var organizer = new Organizer
            {
                Id = "Org_1",
                Email = "login@example.com",
                Name = "Login Test"
            };
            organizer.PasswordHash = _passwordHasher.HashPassword(organizer, "ValidPassword");
            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            var dto = new OrganizerLoginDto
            {
                Email = "login@example.com",
                Password = "ValidPassword"
            };

            var expectedToken = "mock_login_token";
            _mockJwtTokenService.Setup(x => x.GenerateToken(It.IsAny<Organizer>())).Returns(expectedToken);

            // Act
            var result = await _authService.LoginAsync(dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("login@example.com", result.Email);
            Assert.AreEqual(expectedToken, result.Token);
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var organizer = new Organizer
            {
                Id = "Org_2",
                Email = "wrong@example.com"
            };
            organizer.PasswordHash = _passwordHasher.HashPassword(organizer, "CorrectPassword");
            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            var dto = new OrganizerLoginDto
            {
                Email = "wrong@example.com",
                Password = "WrongPassword"
            };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(dto, CancellationToken.None));
        }

        [Test]
        public void LoginAsync_NonExistentEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var dto = new OrganizerLoginDto
            {
                Email = "doesnotexist@example.com",
                Password = "AnyPassword"
            };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(dto, CancellationToken.None));
        }
    }
}
