using Data;
using Entities.Dtos.Appearance;
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
    public class AppearanceServiceTests
    {
        private AppDbContext _context;
        private AppearanceService _appearanceService;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);
            _appearanceService = new AppearanceService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ValidInput_CreatesAppearance()
        {
            var dto = new AppearanceCreateDto { Name = "My Theme", PrimaryColor = "#fff", IsDefault = false };
            var result = await _appearanceService.CreateAsync("Org_1", dto, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual("My Theme", result.Name);

            var dbItem = await _context.Appearances.FindAsync(result.Id);
            Assert.IsNotNull(dbItem);
            Assert.AreEqual("My Theme", dbItem.Name);
        }

        [Test]
        public async Task CreateAsync_SetAsDefault_ResetsOtherDefaults()
        {
            _context.Appearances.Add(new Appearance { Id = "App_1", OrganizerId = "Org_1", Name = "Old Default", IsDefault = true });
            await _context.SaveChangesAsync();

            var dto = new AppearanceCreateDto { Name = "New Default", IsDefault = true };
            var result = await _appearanceService.CreateAsync("Org_1", dto, CancellationToken.None);

            var oldDefault = await _context.Appearances.FindAsync("App_1");
            Assert.IsFalse(oldDefault.IsDefault);
            
            var newDefault = await _context.Appearances.FindAsync(result.Id);
            Assert.IsTrue(newDefault.IsDefault);
        }

        [Test]
        public async Task DeleteAsync_LastTheme_ReturnsIsLastTheme()
        {
            _context.Appearances.Add(new Appearance { Id = "App_1", OrganizerId = "Org_1", Name = "Last Theme" });
            await _context.SaveChangesAsync();

            var result = await _appearanceService.DeleteAsync("App_1", CancellationToken.None);

            Assert.AreEqual(DeleteAppearanceResult.IsLastTheme, result);
            var dbItem = await _context.Appearances.FindAsync("App_1");
            Assert.IsNotNull(dbItem); // Should not be deleted
        }

        [Test]
        public async Task DeleteAsync_HasOtherThemes_DeletesAndReturnsSuccess()
        {
            _context.Appearances.Add(new Appearance { Id = "App_1", OrganizerId = "Org_1", Name = "Theme 1", IsDefault = true });
            _context.Appearances.Add(new Appearance { Id = "App_2", OrganizerId = "Org_1", Name = "Theme 2", IsDefault = false });
            await _context.SaveChangesAsync();

            var result = await _appearanceService.DeleteAsync("App_2", CancellationToken.None);

            Assert.AreEqual(DeleteAppearanceResult.Success, result);
            Assert.IsNull(await _context.Appearances.FindAsync("App_2"));
        }
    }
}
