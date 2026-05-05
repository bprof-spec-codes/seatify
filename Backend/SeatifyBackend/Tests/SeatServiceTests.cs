using Data;
using Entities.Dtos.Exceptions;
using Entities.Dtos.Seat;
using Entities.Dtos.Venue;
using Entities.Models;
using Logic.Helper;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class SeatServiceTests
    {
        private AppDbContext _context;
        private SeatService _seatService;

        [SetUp]
        public void Setup()
        {
            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

            _context = new AppDbContext(options);
            _seatService = new SeatService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public void CreateBatchAsync_NoDtosProvided_ThrowsArgumentException()
        {
            SeatViewDto dto = new SeatViewDto();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _seatService.CreateBatchAsync(null, CancellationToken.None));
            Assert.AreEqual("At least one seat must be provided.", ex.Message);
        }

        [Test]
        public void CreateBatchAsync_MissingMatrixId_ThrowsArgumentException()
        {
            // Arrange
            SeatViewDto dto = new SeatViewDto();
            dto.MatrixId = string.Empty;
            dto.SeatType = "Seat";

            List<SeatViewDto> dtos = new List<SeatViewDto> { dto };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _seatService.CreateBatchAsync(dtos, CancellationToken.None));
            Assert.AreEqual("MatrixId is required.", ex.Message);
        }

        [Test]
        public async Task CreateBatchAsync_ValidInput_CreatesSeatsAndReturnsList()
        {
            // Arrange
            Auditorium auditorium = new Auditorium();
            auditorium.Id = "Aud_Batch";
            auditorium.Name = "Test Aud";

            LayoutMatrix matrix = new LayoutMatrix();
            matrix.Id = "Mat_Batch";
            matrix.AuditoriumId = "Aud_Batch";
            matrix.Name = "Main Matrix";

            _context.Auditoriums.Add(auditorium);
            _context.LayoutMatrices.Add(matrix);
            await _context.SaveChangesAsync();

            SeatViewDto dto1 = new SeatViewDto();
            dto1.MatrixId = "Mat_Batch";
            dto1.Row = 1;
            dto1.Column = 1;
            dto1.SeatType = "Seat";

            SeatViewDto dto2 = new SeatViewDto();
            dto2.MatrixId = "Mat_Batch";
            dto2.Row = 1;
            dto2.Column = 2;
            dto2.SeatType = "AccessibleSeat";

            List<SeatViewDto> dtos = new List<SeatViewDto> { dto1, dto2 };

            // Act
            var result = await _seatService.CreateBatchAsync(dtos, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            var dbSeats = await _context.Seats.ToListAsync();
            Assert.AreEqual(2, dbSeats.Count);
            Assert.AreEqual(SeatType.AccessibleSeat, dbSeats.First(s => s.Column == 2).SeatType);
        }

        // --- GetAllAsync Tests ---

        [Test]
        public async Task GetAllAsync_ReturnsAllSeatsOrdered()
        {
            // Arrange
            LayoutMatrix matrix = new LayoutMatrix();
            matrix.Id = "Mat_GetAll";
            _context.LayoutMatrices.Add(matrix);

            Seat seat1 = new Seat();
            seat1.Id = "S2";
            seat1.MatrixId = "Mat_GetAll";
            seat1.Row = 2;
            seat1.Column = 1;
            seat1.SeatType = SeatType.Seat;

            Seat seat2 = new Seat();
            seat2.Id = "S1";
            seat2.MatrixId = "Mat_GetAll";
            seat2.Row = 1;
            seat2.Column = 1;
            seat2.SeatType = SeatType.Seat;

            _context.Seats.Add(seat1);
            _context.Seats.Add(seat2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _seatService.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("S1", result[0].Id); // Row 1 should come before Row 2
            Assert.AreEqual("S2", result[1].Id);
        }

        // --- GetByMatrixAsync Tests ---

        [Test]
        public void GetByMatrixAsync_MatrixNotFound_ThrowsArgumentException()
        {
            // Arrange
            // Empty database

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _seatService.GetByMatrixAsync("InvalidMatrix", CancellationToken.None));
            Assert.AreEqual("LayoutMatrix not found.", ex.Message);
        }

        [Test]
        public async Task GetByMatrixAsync_ValidMatrixId_ReturnsFilteredSeats()
        {
            // Arrange
            LayoutMatrix targetMatrix = new LayoutMatrix();
            targetMatrix.Id = "Mat_Target";

            LayoutMatrix otherMatrix = new LayoutMatrix();
            otherMatrix.Id = "Mat_Other";

            _context.LayoutMatrices.Add(targetMatrix);
            _context.LayoutMatrices.Add(otherMatrix);

            Seat seat1 = new Seat();
            seat1.Id = "Seat_Target";
            seat1.MatrixId = "Mat_Target";
            seat1.Row = 1;
            seat1.Column = 1;

            Seat seat2 = new Seat();
            seat2.Id = "Seat_Other";
            seat2.MatrixId = "Mat_Other";
            seat2.Row = 1;
            seat2.Column = 1;

            _context.Seats.Add(seat1);
            _context.Seats.Add(seat2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _seatService.GetByMatrixAsync("Mat_Target", CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Seat_Target", result[0].Id);
        }

        // --- GetByIdAsync Tests ---

        [Test]
        public async Task GetByIdAsync_SeatNotFound_ReturnsNull()
        {
            // Arrange
            // Empty database

            // Act
            var result = await _seatService.GetByIdAsync("MissingId", CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_ValidSeatId_ReturnsSeat()
        {
            // Arrange
            Seat seat = new Seat();
            seat.Id = "Seat_FindMe";
            seat.MatrixId = "Mat_1";
            seat.Row = 5;
            seat.Column = 5;
            seat.SeatType = SeatType.AccessibleSeat;

            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            // Act
            var result = await _seatService.GetByIdAsync("Seat_FindMe", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Seat_FindMe", result.Id);
            Assert.AreEqual(5, result.Row);
            Assert.AreEqual("AccessibleSeat", result.SeatType);
        }

        // --- BulkUpdateAsync Tests ---

        [Test]
        public void BulkUpdateAsync_NoSeatIds_ThrowsArgumentException()
        {
            // Arrange
            BulkSeatUpdateDto dto = new BulkSeatUpdateDto();
            dto.SeatIds = new List<string>();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _seatService.BulkUpdateAsync(dto, CancellationToken.None));
            Assert.AreEqual("At least one SeatId must be provided.", ex.Message);
        }

        [Test]
        public async Task BulkUpdateAsync_ValidInput_UpdatesSeatsAndReturnsResponse()
        {
            // Arrange
            LayoutMatrix matrix = new LayoutMatrix();
            matrix.Id = "Mat_Bulk";
            _context.LayoutMatrices.Add(matrix);

            Seat seat1 = new Seat();
            seat1.Id = "S1_Bulk";
            seat1.MatrixId = "Mat_Bulk";
            seat1.Row = 1;
            seat1.Column = 1;
            seat1.SeatType = SeatType.Seat;

            Seat seat2 = new Seat();
            seat2.Id = "S2_Bulk";
            seat2.MatrixId = "Mat_Bulk";
            seat2.Row = 1;
            seat2.Column = 2;
            seat2.SeatType = SeatType.Seat;

            _context.Seats.Add(seat1);
            _context.Seats.Add(seat2);
            await _context.SaveChangesAsync();

            BulkSeatUpdateDto dto = new BulkSeatUpdateDto();
            dto.SeatIds = new List<string> { "S1_Bulk", "S2_Bulk" };
            dto.SeatType = "Aisle";
            dto.PriceOverride = 5000;

            // Act
            var result = await _seatService.BulkUpdateAsync(dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.UpdatedCount);

            var updatedSeats = await _context.Seats.ToListAsync();
            Assert.AreEqual(SeatType.Aisle, updatedSeats[0].SeatType);
            Assert.AreEqual(5000, updatedSeats[0].PriceOverride);
            Assert.AreEqual(SeatType.Aisle, updatedSeats[1].SeatType);
        }

        // --- UpdateAsync Tests ---

        [Test]
        public void UpdateAsync_MissingSeatId_ThrowsArgumentException()
        {
            // Arrange
            SeatUpdateDto dto = new SeatUpdateDto();
            dto.SeatType = "Seat";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _seatService.UpdateAsync(string.Empty, dto, CancellationToken.None));
            Assert.AreEqual("SeatId is required.", ex.Message);
        }

        [Test]
        public async Task UpdateAsync_ValidInput_UpdatesSeatAndReturnsDto()
        {
            // Arrange
            Seat seat = new Seat();
            seat.Id = "Seat_Update";
            seat.MatrixId = "Mat_1";
            seat.SeatType = SeatType.Seat;
            seat.SeatLabel = "OldLabel";
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            SeatUpdateDto dto = new SeatUpdateDto();
            dto.SeatType = "AccessibleSeat";
            dto.SeatLabel = "NewLabel";
            dto.PriceOverride = 1500;

            // Act
            var result = await _seatService.UpdateAsync("Seat_Update", dto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AccessibleSeat", result.SeatType);
            Assert.AreEqual("NewLabel", result.SeatLabel);
            Assert.AreEqual(1500, result.PriceOverride);

            var dbSeat = await _context.Seats.FindAsync("Seat_Update");
            Assert.AreEqual(SeatType.AccessibleSeat, dbSeat.SeatType);
        }

        // --- DeleteAsync Tests ---

        [Test]
        public async Task DeleteAsync_SeatNotFound_ReturnsFalse()
        {
            // Arrange
            // Empty database

            // Act
            var result = await _seatService.DeleteAsync("NonExistentSeat", CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteAsync_ValidSeatId_DeletesSeatAndReturnsTrue()
        {
            // Arrange
            Seat seat = new Seat();
            seat.Id = "Seat_Delete";
            seat.MatrixId = "Mat_1";
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            // Act
            var result = await _seatService.DeleteAsync("Seat_Delete", CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            var dbSeat = await _context.Seats.FindAsync("Seat_Delete");
            Assert.IsNull(dbSeat);
        }

        // --- GetSeatAvailability Tests ---

        [Test]
        public void GetSeatAvailability_NullRequest_ThrowsArgumentException()
        {
            // Arrange
            SeatAvailabilityRequestDto request = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _seatService.GetSeatAvailability(request));
            Assert.AreEqual("Request is required.", ex.Message);
        }

        [Test]
        public void GetSeatAvailability_OccurrenceNotFound_ThrowsEventNotFoundException()
        {
            // Arrange
            SeatAvailabilityRequestDto request = new SeatAvailabilityRequestDto();
            request.eventOccurrenceId = "MissingOcc";
            request.seatIds = new List<string> { "S1" };

            // Act & Assert
            var ex = Assert.Throws<EventNotFoundException>(() => _seatService.GetSeatAvailability(request));
            Assert.IsTrue(ex.Message.Contains("could not be found"));
        }
    }
}
