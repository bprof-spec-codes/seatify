using Data;
using Entities.Dtos.CheckIn;
using Entities.Models;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests
{
    public class CheckInServiceTests
    {
        private AppDbContext _context;
        private CheckInService _checkInService;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);
            _checkInService = new CheckInService(_context);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task ValidateTicketAsync_ValidTicket_ReturnsValidStatus()
        {
            // Arrange
            var rsId = "RS_1";
            var reservation = new Reservation { Id = "Res_1", CustomerName = "John Doe", EventOccurrence = new EventOccurrence { Event = new Event { Name = "Concert" } } };
            var seat = new Seat { Id = "Seat_1", Row = 1, Column = 1, Sector = new Sector { Name = "VIP" } };
            var rs = new ReservationSeat { Id = rsId, ReservationId = reservation.Id, SeatId = seat.Id, Reservation = reservation, Seat = seat, IsCheckedIn = false };
            
            _context.Reservations.Add(reservation);
            _context.Seats.Add(seat);
            _context.ReservationSeats.Add(rs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.ValidateTicketAsync($"Ticket:{rsId}");

            // Assert
            Assert.AreEqual(TicketStatus.Valid, result.Status);
            Assert.AreEqual(rsId, result.TicketId);
            Assert.AreEqual("John Doe", result.Reservation.CustomerName);
            Assert.AreEqual("VIP", result.Seat.Section);
        }

        [Test]
        public async Task ValidateTicketAsync_AlreadyCheckedIn_ReturnsAlreadyUsedStatus()
        {
            // Arrange
            var rsId = "RS_2";
            var reservation = new Reservation { Id = "Res_2", CustomerName = "Jane Doe", EventOccurrence = new EventOccurrence { Event = new Event { Name = "Concert" } } };
            var seat = new Seat { Id = "Seat_2", Row = 1, Column = 2, Sector = new Sector { Name = "VIP" } };
            var rs = new ReservationSeat { Id = rsId, ReservationId = reservation.Id, SeatId = seat.Id, Reservation = reservation, Seat = seat, IsCheckedIn = true, CheckInTimeUtc = DateTime.UtcNow };
            
            _context.Reservations.Add(reservation);
            _context.Seats.Add(seat);
            _context.ReservationSeats.Add(rs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.ValidateTicketAsync(rsId);

            // Assert
            Assert.AreEqual(TicketStatus.AlreadyUsed, result.Status);
        }

        [Test]
        public async Task ValidateTicketAsync_InvalidTicket_ReturnsInvalidStatus()
        {
            // Act
            var result = await _checkInService.ValidateTicketAsync("Ticket:Invalid_Id");

            // Assert
            Assert.AreEqual(TicketStatus.Invalid, result.Status);
        }

        [Test]
        public async Task ConfirmCheckInAsync_ValidTicket_ChecksInAndReturnsSuccess()
        {
            // Arrange
            var rsId = "RS_3";
            var reservation = new Reservation { Id = "Res_3", CustomerName = "Bob", EventOccurrence = new EventOccurrence { Event = new Event { Name = "Concert" } } };
            var seat = new Seat { Id = "Seat_3", Row = 1, Column = 3, Sector = new Sector { Name = "General" } };
            var rs = new ReservationSeat { Id = rsId, ReservationId = reservation.Id, SeatId = seat.Id, Reservation = reservation, Seat = seat, IsCheckedIn = false };
            
            _context.Reservations.Add(reservation);
            _context.Seats.Add(seat);
            _context.ReservationSeats.Add(rs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.ConfirmCheckInAsync(rsId);

            // Assert
            Assert.AreEqual(TicketStatus.Valid, result.Status); // Original status is Valid, but the DB gets updated
            var dbRs = await _context.ReservationSeats.FindAsync(rsId);
            Assert.IsTrue(dbRs.IsCheckedIn);
            Assert.IsNotNull(dbRs.CheckInTimeUtc);
        }
    }
}
