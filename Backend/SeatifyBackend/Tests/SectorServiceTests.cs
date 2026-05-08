using Data;
using Entities.Dtos.Auditorium;
using Entities.Dtos.Sector;
using Entities.Models;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class SectorServiceTests
    {
        private AppDbContext _context;
        private SectorService _sectorService;

        [SetUp]
        public void Setup()
        {
            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

            _context = new AppDbContext(options);
            _sectorService = new SectorService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }



        [Test]
        public void CreateAsync_MissingAuditorium_ThrowsArgumentException()
        {
            // Arrange
            SectorCreateUpdateDto sectorCreateUpdateDto = new SectorCreateUpdateDto();
            sectorCreateUpdateDto.BasePrice = 10;
            sectorCreateUpdateDto.Name = "Testname";
            sectorCreateUpdateDto.Color = "red";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.CreateAsync(string.Empty, sectorCreateUpdateDto, CancellationToken.None));
            Assert.AreEqual("Auditorium with the specified ID does not exist.", ex.Message);
        }


        [Test]
        public void CreateAsync_MissingSectorName_ThrowsArgumentException()
        {
            // Arrange
            var sectorCreateUpdateDto = new SectorCreateUpdateDto
            {
                BasePrice = 10,
                Name = string.Empty, // Empty name to trigger validation error
                Color = "red"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.CreateAsync("Ad1", sectorCreateUpdateDto, CancellationToken.None));
            Assert.AreEqual("Sector name is required.", ex.Message);
        }

        [Test]
        public async Task CreateAsync_DuplicateSectorName_ThrowsArgumentException()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Currency = "HUF";
            auditorium.Name = "TestName";
            auditorium.Description = "Description";
            auditorium.Id = "Ad1";

            _context.Auditoriums.Add(auditorium);

            _context.SaveChanges();



            SectorCreateUpdateDto sectorCreateUpdateDto = new SectorCreateUpdateDto();
            sectorCreateUpdateDto.BasePrice = 10;
            sectorCreateUpdateDto.Name = "Testname";
            sectorCreateUpdateDto.Color = "red";

            await _sectorService.CreateAsync("Ad1", sectorCreateUpdateDto, CancellationToken.None);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.CreateAsync("Ad1", sectorCreateUpdateDto, CancellationToken.None));
            Assert.AreEqual("Sector with this name already exists in this auditorium.", ex.Message);
        }

        [Test]
        public async Task CreateAsync_ValidInput_CreatesSectorAndReturnsDto()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Currency = "HUF";
            auditorium.Name = "TestName";
            auditorium.Description = "Description";
            auditorium.Id = "Ad_ValidTest"; // Unique ID to prevent conflicts with other tests

            _context.Auditoriums.Add(auditorium);
            _context.SaveChanges();

            SectorCreateUpdateDto sectorCreateUpdateDto = new SectorCreateUpdateDto();
            sectorCreateUpdateDto.BasePrice = 1500;
            sectorCreateUpdateDto.Name = "  Valid Sector  "; // Intentionally added spaces to test Trim()
            sectorCreateUpdateDto.Color = "  #FF0000  "; // Testing Trim() here as well

            // Act
            var result = await _sectorService.CreateAsync("Ad_ValidTest", sectorCreateUpdateDto, CancellationToken.None);

            // Assert - Verify the returned DTO
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Id); // Auto-generated ID
            Assert.AreEqual("Ad_ValidTest", result.AuditoriumId);
            Assert.AreEqual("Valid Sector", result.Name); // Spaces should be trimmed
            Assert.AreEqual("#FF0000", result.Color); // Spaces should be trimmed
            Assert.AreEqual(1500, result.BasePrice);

            // Assert - Verify if it was actually saved to the DB
            var savedSector = await _context.Sectors.FindAsync(result.Id);
            Assert.IsNotNull(savedSector);
            Assert.AreEqual("Valid Sector", savedSector.Name);
        }

        [Test]
        public async Task CreateAsync_EmptyColor_DefaultsToWhite()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Currency = "HUF";
            auditorium.Name = "TestName";
            auditorium.Description = "Description";
            auditorium.Id = "Ad_ColorTest";

            _context.Auditoriums.Add(auditorium);
            _context.SaveChanges();

            SectorCreateUpdateDto sectorCreateUpdateDto = new SectorCreateUpdateDto();
            sectorCreateUpdateDto.BasePrice = 1000;
            sectorCreateUpdateDto.Name = "Sector Without Color";
            sectorCreateUpdateDto.Color = string.Empty; // Empty color to trigger the default fallback logic

            // Act
            var result = await _sectorService.CreateAsync("Ad_ColorTest", sectorCreateUpdateDto, CancellationToken.None);

            // Assert
            Assert.AreEqual("#FFFFFF", result.Color); // Should be white based on the fallback logic
        }

        [Test]
        public async Task DeleteAsync_SectorHasSeats_ThrowsArgumentException()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Del1";
            auditorium.Currency = "HUF";
            auditorium.Name = "TestName";
            auditorium.Description = "Description";
            _context.Auditoriums.Add(auditorium);

            Sector sector = new Sector();
            sector.Id = "Sec_Del1";
            sector.AuditoriumId = "Ad_Del1";
            sector.Name = "Test Sector";
            sector.Color = "#FFFFFF";
            sector.BasePrice = 1000;
            sector.CreatedAtUtc = DateTime.UtcNow;
            sector.UpdatedAtUtc = DateTime.UtcNow;
            _context.Sectors.Add(sector);

            Seat seat = new Seat();
            seat.Id = "Seat1";
            seat.SectorId = "Sec_Del1"; // Assign to the sector!
            // Setup other required Seat properties if there are any (e.g. Row, Column, etc.)
            _context.Seats.Add(seat);

            await _context.SaveChangesAsync();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.DeleteAsync("Sec_Del1", CancellationToken.None));
            Assert.AreEqual("Sector cannot be deleted because seats are assigned to it.", ex.Message);
        }

        [Test]
        public void DeleteAsync_SectorDoesNotExist_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.DeleteAsync("NonExistentId", CancellationToken.None));
            Assert.AreEqual("Sector with the specified ID does not exist.", ex.Message);
        }

        [Test]
        public async Task DeleteAsync_ValidSector_DeletesSectorAndReturnsTrue()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Del2";
            auditorium.Currency = "HUF";
            auditorium.Name = "TestName";
            auditorium.Description = "Description";
            _context.Auditoriums.Add(auditorium);

            Sector sector = new Sector();
            sector.Id = "Sec_Del2";
            sector.AuditoriumId = "Ad_Del2";
            sector.Name = "Sector To Delete";
            sector.Color = "#FFFFFF";
            sector.BasePrice = 1000;
            sector.CreatedAtUtc = DateTime.UtcNow;
            sector.UpdatedAtUtc = DateTime.UtcNow;
            _context.Sectors.Add(sector);

            await _context.SaveChangesAsync();

            // Act
            bool result = await _sectorService.DeleteAsync("Sec_Del2", CancellationToken.None);

            // Assert
            Assert.IsTrue(result); // Expecting a true return value

            // Extra check: ensure it was actually removed from the database
            var deletedSector = await _context.Sectors.FindAsync("Sec_Del2");
            Assert.IsNull(deletedSector);
        }

        [Test]
        public async Task GetByAuditoriumAsync_NoSectors_ReturnsEmptyList()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Get1";
            auditorium.Currency = "HUF";
            auditorium.Name = "Empty Auditorium";
            auditorium.Description = "Description";
            _context.Auditoriums.Add(auditorium);

            await _context.SaveChangesAsync();

            // Act
            var result = await _sectorService.GetByAuditoriumAsync("Ad_Get1", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result); // Check if the list is empty
        }

        [Test]
        public async Task GetByAuditoriumAsync_HasSectors_ReturnsOrderedAndFilteredMappedList()
        {
            // Arrange
            Auditorium auditorium1 = new Auditorium();
            auditorium1.Id = "Ad_GetTarget";
            auditorium1.Currency = "HUF";
            auditorium1.Name = "Target Auditorium";
            auditorium1.Description = "Description";
            _context.Auditoriums.Add(auditorium1);

            Auditorium auditorium2 = new Auditorium();
            auditorium2.Id = "Ad_GetOther";
            auditorium2.Currency = "HUF";
            auditorium2.Name = "Other Auditorium";
            auditorium2.Description = "Description";
            _context.Auditoriums.Add(auditorium2);

            // Sector starting with "B" for the target auditorium
            Sector sector1 = new Sector();
            sector1.Id = "Sec_B";
            sector1.AuditoriumId = "Ad_GetTarget";
            sector1.Name = "B Sector";
            sector1.Color = "#000000";
            sector1.BasePrice = 2000;
            sector1.CreatedAtUtc = DateTime.UtcNow;
            sector1.UpdatedAtUtc = DateTime.UtcNow;
            _context.Sectors.Add(sector1);

            // Sector starting with "A" for the target auditorium (this should be first in the list)
            Sector sector2 = new Sector();
            sector2.Id = "Sec_A";
            sector2.AuditoriumId = "Ad_GetTarget";
            sector2.Name = "A Sector";
            sector2.Color = "#FFFFFF";
            sector2.BasePrice = 1000;
            sector2.CreatedAtUtc = DateTime.UtcNow;
            sector2.UpdatedAtUtc = DateTime.UtcNow;
            _context.Sectors.Add(sector2);

            // A sector belonging to a DIFFERENT auditorium (should not be in the result)
            Sector sector3 = new Sector();
            sector3.Id = "Sec_Other";
            sector3.AuditoriumId = "Ad_GetOther";
            sector3.Name = "C Sector";
            sector3.Color = "#FF0000";
            sector3.BasePrice = 3000;
            sector3.CreatedAtUtc = DateTime.UtcNow;
            sector3.UpdatedAtUtc = DateTime.UtcNow;
            _context.Sectors.Add(sector3);

            await _context.SaveChangesAsync();

            // Act
            var result = await _sectorService.GetByAuditoriumAsync("Ad_GetTarget", CancellationToken.None);

            // Assert - Verify filtering
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); // Should only return the two sectors from the target auditorium

            // Assert - Verify ordering ("A" should come first due to OrderBy)
            Assert.AreEqual("A Sector", result[0].Name);
            Assert.AreEqual("Sec_A", result[0].Id);

            Assert.AreEqual("B Sector", result[1].Name);
            Assert.AreEqual("Sec_B", result[1].Id);

            // Assert - Verify DTO mapping (Select logic)
            Assert.AreEqual("#FFFFFF", result[0].Color);
            Assert.AreEqual(1000, result[0].BasePrice);
            Assert.AreEqual("Ad_GetTarget", result[0].AuditoriumId);
        }

        [Test]
        public async Task GetByIdAsync_SectorDoesNotExist_ReturnsNull()
        {
            // Arrange
            // Empty database, so the requested ID won't exist

            // Act
            var result = await _sectorService.GetByIdAsync("NonExistentId", CancellationToken.None);

            // Assert
            Assert.IsNull(result); // FirstOrDefaultAsync should return null
        }

        [Test]
        public async Task GetByIdAsync_SectorExists_ReturnsMappedDto()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_GetId1";
            auditorium.Currency = "HUF";
            auditorium.Name = "Test Auditorium";
            auditorium.Description = "Description";
            _context.Auditoriums.Add(auditorium);

            Sector sector = new Sector();
            sector.Id = "Sec_GetId1";
            sector.AuditoriumId = "Ad_GetId1";
            sector.Name = "Single Sector";
            sector.Color = "#123456";
            sector.BasePrice = 1500;
            sector.CreatedAtUtc = DateTime.UtcNow;
            sector.UpdatedAtUtc = DateTime.UtcNow;
            _context.Sectors.Add(sector);

            await _context.SaveChangesAsync();

            // Act
            var result = await _sectorService.GetByIdAsync("Sec_GetId1", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Sec_GetId1", result.Id);
            Assert.AreEqual("Ad_GetId1", result.AuditoriumId);
            Assert.AreEqual("Single Sector", result.Name);
            Assert.AreEqual("#123456", result.Color);
            Assert.AreEqual(1500, result.BasePrice);
        }

        [Test]
        public void UpdateAsync_SectorDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            SectorCreateUpdateDto dto = new SectorCreateUpdateDto();
            dto.BasePrice = 10;
            dto.Name = "New Name";
            dto.Color = "red";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.UpdateAsync("NonExistentId", dto, CancellationToken.None));
            Assert.AreEqual("Sector with the specified ID does not exist.", ex.Message);
        }

        [Test]
        public async Task UpdateAsync_MissingSectorName_ThrowsArgumentException()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Upd1";
            auditorium.Currency = "HUF";
            auditorium.Name = "Test Name";
            _context.Auditoriums.Add(auditorium);

            Sector sector = new Sector();
            sector.Id = "Sec_Upd1";
            sector.AuditoriumId = "Ad_Upd1";
            sector.Name = "Original Name";
            sector.Color = "blue";
            sector.BasePrice = 1000;
            _context.Sectors.Add(sector);

            await _context.SaveChangesAsync();

            SectorCreateUpdateDto dto = new SectorCreateUpdateDto();
            dto.BasePrice = 1500;
            dto.Name = string.Empty; // Empty name to trigger validation error
            dto.Color = "red";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.UpdateAsync("Sec_Upd1", dto, CancellationToken.None));
            Assert.AreEqual("Sector name is required.", ex.Message);
        }

        [Test]
        public async Task UpdateAsync_DuplicateSectorName_ThrowsArgumentException()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Upd2";
            auditorium.Currency = "HUF";
            auditorium.Name = "Test Name";
            _context.Auditoriums.Add(auditorium);

            // The sector we want to update
            Sector sectorToUpdate = new Sector();
            sectorToUpdate.Id = "Sec_Target";
            sectorToUpdate.AuditoriumId = "Ad_Upd2";
            sectorToUpdate.Name = "First Sector";
            sectorToUpdate.Color = "blue";
            sectorToUpdate.BasePrice = 1000;
            _context.Sectors.Add(sectorToUpdate);

            // Another sector in the same auditorium whose name trying to use
            Sector existingSector = new Sector();
            existingSector.Id = "Sec_Other";
            existingSector.AuditoriumId = "Ad_Upd2";
            existingSector.Name = "Second Sector";
            existingSector.Color = "green";
            existingSector.BasePrice = 2000;
            _context.Sectors.Add(existingSector);

            await _context.SaveChangesAsync();

            SectorCreateUpdateDto dto = new SectorCreateUpdateDto();
            dto.BasePrice = 1500;
            dto.Name = "second sector"; // Lowercase intentionally to test the .ToLower() logic
            dto.Color = "red";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.UpdateAsync("Sec_Target", dto, CancellationToken.None));
            Assert.AreEqual("Sector with this name already exists in this auditorium.", ex.Message);
        }

        [Test]
        public async Task UpdateAsync_ValidInput_UpdatesSectorAndReturnsDto()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Upd3";
            auditorium.Currency = "HUF";
            auditorium.Name = "Test Name";
            _context.Auditoriums.Add(auditorium);

            Sector sector = new Sector();
            sector.Id = "Sec_Upd3";
            sector.AuditoriumId = "Ad_Upd3";
            sector.Name = "Old Name";
            sector.Color = "blue";
            sector.BasePrice = 1000;
            _context.Sectors.Add(sector);

            await _context.SaveChangesAsync();

            SectorCreateUpdateDto dto = new SectorCreateUpdateDto();
            dto.BasePrice = 5000;
            dto.Name = "  Updated Name  "; // With spaces to test Trim()
            dto.Color = "  #00FF00  "; // With spaces to test Trim()

            // Act
            var result = await _sectorService.UpdateAsync("Sec_Upd3", dto, CancellationToken.None);

            // Assert - Verify DTO
            Assert.IsNotNull(result);
            Assert.AreEqual("Sec_Upd3", result.Id);
            Assert.AreEqual("Updated Name", result.Name); // Space trimmed
            Assert.AreEqual("#00FF00", result.Color); // Space trimmed
            Assert.AreEqual(5000, result.BasePrice);

            // Assert - Verify database
            var updatedSector = await _context.Sectors.FindAsync("Sec_Upd3");
            Assert.IsNotNull(updatedSector);
            Assert.AreEqual("Updated Name", updatedSector.Name);
            Assert.AreEqual(5000, updatedSector.BasePrice);
        }

        [Test]
        public async Task UpdateAsync_EmptyColor_DefaultsToWhite()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Ad_Upd4";
            auditorium.Currency = "HUF";
            auditorium.Name = "Test Name";
            _context.Auditoriums.Add(auditorium);

            Sector sector = new Sector();
            sector.Id = "Sec_Upd4";
            sector.AuditoriumId = "Ad_Upd4";
            sector.Name = "Name";
            sector.Color = "blue"; // Has an original color
            sector.BasePrice = 1000;
            _context.Sectors.Add(sector);

            await _context.SaveChangesAsync();

            SectorCreateUpdateDto dto = new SectorCreateUpdateDto();
            dto.BasePrice = 1000;
            dto.Name = "Name";
            dto.Color = "   "; // Empty string to trigger the default fallback logic

            // Act
            var result = await _sectorService.UpdateAsync("Sec_Upd4", dto, CancellationToken.None);

            // Assert
            Assert.AreEqual("#FFFFFF", result.Color);

            var updatedSector = await _context.Sectors.FindAsync("Sec_Upd4");
            Assert.AreEqual("#FFFFFF", updatedSector.Color);
        }

    }
}