using Data;
using Entities.Dtos.LayoutMatrix;
using Entities.Models;
using Logic.Helper;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class LayoutMatrixServiceTests
    {
        private AppDbContext _dbContext;
        private LayoutMatrixService _layoutMatrixService;
        private DtoProvider _dtoProvider;

        [SetUp]
        public void SetUp()
        {
            // Create a unique in-memory database for each test to ensure isolation
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _dtoProvider = new DtoProvider();
            _layoutMatrixService = new LayoutMatrixService(_dbContext, _dtoProvider);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private async Task SeedAuditoriumAsync(string auditoriumId)
        {
            var auditorium = new Auditorium { Id = auditoriumId, Name = "Test Auditorium", VenueId = "V1" };
            _dbContext.Auditoriums.Add(auditorium);
            await _dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CreateAsync_ValidInput_CreatesMatrixAndSeats()
        {
            // Arrange
            string auditoriumId = "Aud-1";
            await SeedAuditoriumAsync(auditoriumId);

            var dto = new LayoutMatrixCreateDto
            {
                Name = "Main Matrix",
                Rows = 2,
                Columns = 3
            };

            // Act
            var result = await _layoutMatrixService.CreateAsync(auditoriumId, dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.Rows, result.Rows);
            Assert.AreEqual(dto.Columns, result.Columns);

            // Verify database state
            var savedMatrix = await _dbContext.LayoutMatrices.Include(lm => lm.Seats).FirstOrDefaultAsync(lm => lm.Id == result.Id);
            Assert.IsNotNull(savedMatrix);

            // 2 rows * 3 columns = 6 seats
            Assert.AreEqual(6, savedMatrix.Seats.Count);

            // Verify specific seat generation (Row 1 = A, Column 1 = 1 -> A1)
            Assert.IsTrue(savedMatrix.Seats.Any(s => s.Row == 1 && s.Column == 1 && s.SeatLabel == "A1"));
            Assert.IsTrue(savedMatrix.Seats.Any(s => s.Row == 2 && s.Column == 3 && s.SeatLabel == "B3"));
        }

        [Test]
        public void CreateAsync_NonExistingAuditorium_ThrowsKeyNotFoundException()
        {
            // Arrange
            var dto = new LayoutMatrixCreateDto { Name = "Matrix 1", Rows = 10, Columns = 10 };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _layoutMatrixService.CreateAsync("Invalid-Auditorium", dto, CancellationToken.None));
            Assert.AreEqual("Auditorium not found.", ex.Message);
        }

        [Test]
        public async Task CreateAsync_DuplicateNameInSameAuditorium_ThrowsInvalidOperationException()
        {
            // Arrange
            string auditoriumId = "Aud-1";
            await SeedAuditoriumAsync(auditoriumId);

            var existingMatrix = new LayoutMatrix { AuditoriumId = auditoriumId, Name = "Existing Matrix", Rows = 5, Columns = 5 };
            _dbContext.LayoutMatrices.Add(existingMatrix);
            await _dbContext.SaveChangesAsync();

            var dto = new LayoutMatrixCreateDto { Name = "existing matrix", Rows = 10, Columns = 10 };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _layoutMatrixService.CreateAsync(auditoriumId, dto, CancellationToken.None));
            Assert.AreEqual("A layout matrix with this name already exists in this auditorium.", ex.Message);
        }

        [Test]
        public async Task DeleteAsync_ExistingId_DeletesMatrixAndAssociatedSeats()
        {
            // Arrange
            var matrix = new LayoutMatrix { Id = "LM-1", AuditoriumId = "Aud-1", Name = "Matrix", Rows = 1, Columns = 1 };
            matrix.Seats.Add(new Seat { Id = "S1", MatrixId = "LM-1", Row = 1, Column = 1, SeatLabel = "A1" });

            _dbContext.LayoutMatrices.Add(matrix);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _layoutMatrixService.DeleteAsync(matrix.Id, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, await _dbContext.LayoutMatrices.CountAsync());
            Assert.AreEqual(0, await _dbContext.Seats.CountAsync()); // Ensure seats are deleted too
        }

        [Test]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _layoutMatrixService.DeleteAsync("invalid-id", CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllByAuditoriumIdAsync_ReturnsOnlyMatricesForGivenAuditorium()
        {
            // Arrange
            var matrix1 = new LayoutMatrix { Id = "LM-1", AuditoriumId = "Aud-1", Name = "A Matrix" };
            var matrix2 = new LayoutMatrix { Id = "LM-2", AuditoriumId = "Aud-1", Name = "B Matrix" };
            var matrix3 = new LayoutMatrix { Id = "LM-3", AuditoriumId = "Aud-2", Name = "Other Matrix" };

            _dbContext.LayoutMatrices.AddRange(matrix1, matrix2, matrix3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _layoutMatrixService.GetAllByAuditoriumIdAsync("Aud-1", CancellationToken.None);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(m => m.AuditoriumId == "Aud-1"));
            Assert.AreEqual("A Matrix", result[0].Name); // Checks ordering
        }

        [Test]
        public async Task GetSeatMapByIdAsync_ExistingId_ReturnsOrderedSeats()
        {
            // Arrange
            string audId = "Aud-Order";
            await SeedAuditoriumAsync(audId);
            
            var matrix = new LayoutMatrix { Id = "LM-1", AuditoriumId = audId, Name = "Matrix" };
            // Add seats out of order
            matrix.Seats.Add(new Seat { Id = "S1", MatrixId = "LM-1", Row = 2, Column = 1, SeatLabel = "B1" });
            matrix.Seats.Add(new Seat { Id = "S2", MatrixId = "LM-1", Row = 1, Column = 2, SeatLabel = "A2" });
            matrix.Seats.Add(new Seat { Id = "S3", MatrixId = "LM-1", Row = 1, Column = 1, SeatLabel = "A1" });

            _dbContext.LayoutMatrices.Add(matrix);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _layoutMatrixService.GetSeatMapByIdAsync(matrix.Id, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Seats.Count);

            // Verify ordering (Row then Column)
            Assert.AreEqual("A1", result.Seats.ElementAt(0).SeatLabel);
            Assert.AreEqual("A2", result.Seats.ElementAt(1).SeatLabel);
            Assert.AreEqual("B1", result.Seats.ElementAt(2).SeatLabel);
        }

        [Test]
        public async Task UpdateAsync_ExpandingDimensions_AddsNewSeats()
        {
            // Arrange
            var matrix = new LayoutMatrix { Id = "LM-1", AuditoriumId = "Aud-1", Name = "Matrix", Rows = 1, Columns = 1 };
            matrix.Seats.Add(new Seat { MatrixId = "LM-1", Row = 1, Column = 1, SeatLabel = "A1" });

            _dbContext.LayoutMatrices.Add(matrix);
            await _dbContext.SaveChangesAsync();

            var updateDto = new LayoutMatrixUpdateDto { Name = "Expanded Matrix", Rows = 2, Columns = 2 };

            // Act
            var result = await _layoutMatrixService.UpdateAsync(matrix.Id, updateDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Rows);
            Assert.AreEqual(2, result.Columns);

            var dbSeats = await _dbContext.Seats.Where(s => s.MatrixId == matrix.Id).ToListAsync();
            Assert.AreEqual(4, dbSeats.Count); // 1x1 -> 2x2 means 4 seats total
            Assert.IsTrue(dbSeats.Any(s => s.SeatLabel == "B2"));
        }

        [Test]
        public async Task UpdateAsync_ShrinkingDimensions_RemovesOutOfBoundsSeats()
        {
            // Arrange
            var matrix = new LayoutMatrix { Id = "LM-1", AuditoriumId = "Aud-1", Name = "Matrix", Rows = 2, Columns = 2 };
            matrix.Seats.Add(new Seat { MatrixId = "LM-1", Row = 1, Column = 1, SeatLabel = "A1" });
            matrix.Seats.Add(new Seat { MatrixId = "LM-1", Row = 1, Column = 2, SeatLabel = "A2" });
            matrix.Seats.Add(new Seat { MatrixId = "LM-1", Row = 2, Column = 1, SeatLabel = "B1" });
            matrix.Seats.Add(new Seat { MatrixId = "LM-1", Row = 2, Column = 2, SeatLabel = "B2" });

            _dbContext.LayoutMatrices.Add(matrix);
            await _dbContext.SaveChangesAsync();

            var updateDto = new LayoutMatrixUpdateDto { Name = "Shrunk Matrix", Rows = 1, Columns = 1 };

            // Act
            var result = await _layoutMatrixService.UpdateAsync(matrix.Id, updateDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);

            var dbSeats = await _dbContext.Seats.Where(s => s.MatrixId == matrix.Id).ToListAsync();
            Assert.AreEqual(1, dbSeats.Count); // 2x2 -> 1x1 means only 1 seat remains
            Assert.AreEqual("A1", dbSeats.First().SeatLabel);
        }

        [Test]
        public void UpdateAsync_DuplicateName_ThrowsInvalidOperationException()
        {
            // Arrange
            _dbContext.LayoutMatrices.Add(new LayoutMatrix { Id = "LM-1", AuditoriumId = "Aud-1", Name = "Target Matrix", Rows = 1, Columns = 1 });
            _dbContext.LayoutMatrices.Add(new LayoutMatrix { Id = "LM-2", AuditoriumId = "Aud-1", Name = "Existing Name", Rows = 1, Columns = 1 });
            _dbContext.SaveChanges();

            var updateDto = new LayoutMatrixUpdateDto { Name = "existing name", Rows = 1, Columns = 1 };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _layoutMatrixService.UpdateAsync("LM-1", updateDto, CancellationToken.None));
            Assert.AreEqual("A layout matrix with this name already exists in this auditorium.", ex.Message);
        }
    }
}