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
            string dbName = "testDB";
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
        public async Task CreateAsync_MissingAuditorium_ThrowsArgumentException()
        {
            // Arrange
            SectorCreateUpdateDto sectorCreateUpdateDto = new SectorCreateUpdateDto();
            sectorCreateUpdateDto.BasePrice = 10;
            sectorCreateUpdateDto.Name = "Testname";
            sectorCreateUpdateDto.Color = "red";

            
            Assert.ThrowsAsync<ArgumentException>(async() => await _sectorService.CreateAsync(string.Empty, sectorCreateUpdateDto, CancellationToken.None));
        }

        //TODO CreateAsync_MissingSectorName_ThrowsArgumentException 
        /*
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

            await _sectorService.CreateAsync("Ad1", sectorCreateUpdateDto, CancellationToken.None));

            //szektor

            Assert.ThrowsAsync<ArgumentException>(async () => await _sectorService.CreateAsync("Ad1", sectorCreateUpdateDto, CancellationToken.None));
        }
        */
    }
}