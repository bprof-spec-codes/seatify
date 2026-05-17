using Entities.Models;
using Logic.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class JwtTokenServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private JwtTokenService _service;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "SuperSecretKeyThatIsAtLeast32BytesLong1234!"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:AccessTokenMinutes", "60"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new JwtTokenService(configuration);
        }

        [Test]
        public void GenerateToken_ValidOrganizer_ReturnsTokenString()
        {
            var organizer = new Organizer { Id = "Org_1", Email = "test@example.com", Name = "Test User" };

            var token = _service.GenerateToken(organizer);

            Assert.IsFalse(string.IsNullOrEmpty(token));
            // A valid JWT string has two dots
            Assert.AreEqual(2, token.Split('.').Length - 1);
        }

        [Test]
        public void GetExpiryUtc_ReturnsFutureDateTime()
        {
            var expiry = _service.GetExpiryUtc();

            Assert.IsTrue(expiry > System.DateTime.UtcNow);
        }
    }
}
