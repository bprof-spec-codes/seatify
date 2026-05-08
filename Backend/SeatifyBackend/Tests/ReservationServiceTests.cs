using Data;
using Entities.Dtos.Bookings;
using Entities.Dtos.Exceptions;
using Entities.Dtos.Reservation;
using Entities.Models;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class ReservationServiceTests
    {
        private AppDbContext _context;
        private Mock<QrService> _mockQrService;
        private Mock<IEmailService> _mockEmailService;
        private ReservationService _reservationService;

        [SetUp]
        public void Setup()
        {
            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new AppDbContext(options);

            _mockQrService = new Mock<QrService>();
            _mockEmailService = new Mock<IEmailService>();

            _reservationService = new ReservationService(_context, _mockQrService.Object, _mockEmailService.Object);
        }

        [TearDown]
        public void Cooldown()
        {
            _context.Dispose();
        }

        // --------------------------------------------------------------------------
        // CreateReservation Tests
        // --------------------------------------------------------------------------

        [Test]
        public void CreateReservation_ValidInput_CreatesReservationAndReturnsTrue()
        {
            // Arrange
            var dto = new ReservationCreateDto
            {
                CustomerName = "John Doe",
                CustomerEmail = "john@example.com",
                CustomerPhone = "123456789",
                Seats = new List<ReservationSeatDto>
                {
                    new ReservationSeatDto { SeatId = "Seat_1" }
                }
            };
            string eventOccurrenceId = "Occ_1";

            // Act
            var result = _reservationService.CreateReservation(eventOccurrenceId, dto);

            // Assert
            Assert.IsTrue(result);
            var savedReservation = _context.Reservations.Include(r => r.ReservationSeats).FirstOrDefault();
            Assert.IsNotNull(savedReservation);
            Assert.AreEqual("John Doe", savedReservation.CustomerName);
            Assert.AreEqual("Confirmed", savedReservation.Status);
            Assert.AreEqual(1, savedReservation.ReservationSeats.Count);
        }

        // --------------------------------------------------------------------------
        // GetById Tests
        // --------------------------------------------------------------------------

        [Test]
        public void GetById_NonExistingId_ReturnsNull()
        {
            // Arrange
            string nonExistingId = "Invalid_Id";

            // Act
            var result = _reservationService.GetById(nonExistingId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetById_ExistingId_ReturnsReservationViewDto()
        {
            // Arrange
            var reservation = new Reservation
            {
                Id = "Res_1",
                CustomerName = "Jane Doe",
                CustomerEmail = "jane@example.com",
                Status = "Confirmed",
                CreatedAtUtc = DateTime.UtcNow,
                ReservationSeats = new List<ReservationSeat>
                {
                    new ReservationSeat { SeatId = "Seat_1", FinalPrice = 5000 }
                }
            };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // Act
            var result = _reservationService.GetById("Res_1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Res_1", result.Id);
            Assert.AreEqual("Jane Doe", result.CustomerName);
            Assert.AreEqual(1, result.ReservedSeats.Count);
            Assert.AreEqual(5000, result.ReservedSeats.First().FinalPrice);
        }

        // --------------------------------------------------------------------------
        // GetByOccurrenceId Tests
        // --------------------------------------------------------------------------

        [Test]
        public void GetByOccurrenceId_ReturnsFilteredReservations()
        {
            // Arrange
            string targetOccurrenceId = "Occ_Target";

            _context.Reservations.Add(new Reservation { Id = "Res_1", EventOccurrenceId = targetOccurrenceId });
            _context.Reservations.Add(new Reservation { Id = "Res_2", EventOccurrenceId = targetOccurrenceId });
            _context.Reservations.Add(new Reservation { Id = "Res_3", EventOccurrenceId = "Other_Occ" });
            _context.SaveChanges();

            // Act
            var result = _reservationService.GetByOccurrenceId(targetOccurrenceId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(r => r.Id == "Res_1" || r.Id == "Res_2"));
        }

        // --------------------------------------------------------------------------
        // UpdateReservation Tests
        // --------------------------------------------------------------------------

        [Test]
        public void UpdateReservation_NonExistingId_ReturnsFalse()
        {
            // Arrange
            var dto = new ReservationUpdateDto(); // Assuming this DTO exists based on parameter usage

            // Act
            var result = _reservationService.UpdateReservation("Invalid_Id", dto);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateReservation_ExistingId_UpdatesAndReturnsTrue()
        {
            // Arrange
            var reservation = new Reservation
            {
                Id = "Res_Update",
                CustomerName = "Old Name",
                CustomerEmail = "old@example.com",
                CustomerPhone = "000",
                Status = "Pending"
            };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            var dto = new ReservationUpdateDto
            {
                CustomerName = "New Name",
                CustomerEmail = "new@example.com",
                CustomerPhone = "111",
                Status = "Confirmed"
            };

            // Act
            var result = _reservationService.UpdateReservation("Res_Update", dto);

            // Assert
            Assert.IsTrue(result);
            var updatedRes = _context.Reservations.Find("Res_Update");
            Assert.AreEqual("New Name", updatedRes.CustomerName);
            Assert.AreEqual("Confirmed", updatedRes.Status);
        }

        // --------------------------------------------------------------------------
        // DeleteReservation Tests
        // --------------------------------------------------------------------------

        [Test]
        public void DeleteReservation_NonExistingId_ReturnsFalse()
        {
            // Arrange
            string invalidId = "Invalid_Id";

            // Act
            var result = _reservationService.DeleteReservation(invalidId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteReservation_ExistingId_DeletesAndReturnsTrue()
        {
            // Arrange
            var reservation = new Reservation { Id = "Res_Delete" };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // Act
            var result = _reservationService.DeleteReservation("Res_Delete");

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(_context.Reservations.Find("Res_Delete"));
        }

        // --------------------------------------------------------------------------
        // CheckoutReservation Tests
        // --------------------------------------------------------------------------

        [Test]
        public void CheckoutReservation_InvalidBookingSessionId_ThrowsException()
        {
            // Arrange
            var request = new BookingCheckoutRequestDto
            {
                BookingSessionId = "Invalid_Session_Id"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<BookingSessionNotFoundException>(async () =>
                await _reservationService.CheckoutReservation(request));
            Assert.IsTrue(ex.Message.Contains("BookingSession could not be found"));
        }

        [Test]
        public void CheckoutReservation_InvalidEventOccurrenceId_ThrowsException()
        {
            // Arrange
            var request = new BookingCheckoutRequestDto
            {
                EventOccurrenceId = "Invalid_Occ_Id"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<EventOccurrenceNotFoundException>(async () =>
                await _reservationService.CheckoutReservation(request));
            Assert.IsTrue(ex.Message.Contains("EventOccurrence could not be found"));
        }

        [Test]
        public async Task CheckoutReservation_SeatsAlreadyBooked_ThrowsArgumentException()
        {
            // Arrange
            string occId = "Occ_Booked";
            string eventId = "Ev_Booked";
            string audId = "Aud_Booked";

            // 1. Kötelező navigációs entitások létrehozása
            var ev = new Event { Id = eventId };
            var aud = new Auditorium { Id = audId };

            var occ = new EventOccurrence
            {
                Id = occId,
                EventId = eventId,
                AuditoriumId = audId,
                Event = ev,
                Auditorium = aud
            };

            _context.Events.Add(ev);
            _context.Auditoriums.Add(aud);
            _context.EventOccurrences.Add(occ);

            // 2. Add an already confirmed reservation for Seat_1
            var existingReservation = new Reservation
            {
                Id = "Res_Existing",
                EventOccurrenceId = occId,
                Status = "Confirmed"
            };
            existingReservation.ReservationSeats.Add(new ReservationSeat { SeatId = "Seat_1", Reservation = existingReservation });

            _context.Reservations.Add(existingReservation);
            await _context.SaveChangesAsync();

            var request = new BookingCheckoutRequestDto
            {
                EventOccurrenceId = occId,
                SeatIds = new List<string> { "Seat_1" }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reservationService.CheckoutReservation(request));
            Assert.IsTrue(ex.Message.Contains("are already booked"));
        }

        [Test]
        public async Task CheckoutReservation_ValidRequest_CreatesReservationSendsEmailAndReturnsDto()
        {
            // Arrange
            string eventId = "Ev_1";
            string occId = "Occ_1";
            string seatId = "Seat_1";
            string audId = "Aud_1";

            var ev = new Event { Id = eventId, Name = "Test Event", Appearance = new EventAppearance { Currency = "USD" } };
            var aud = new Auditorium { Id = audId, Name = "Main Hall" };

            var occ = new EventOccurrence
            {
                Id = occId,
                EventId = eventId,
                AuditoriumId = audId,
                Event = ev,
                Auditorium = aud,
                StartsAtUtc = DateTime.UtcNow
            };

            _context.Events.Add(ev);
            _context.Auditoriums.Add(aud);
            _context.EventOccurrences.Add(occ);

            // Setup Sector and Seat for pricing logic
            var sector = new Sector { Id = "Sec_1", BasePrice = 1000m, AuditoriumId = audId };
            var seat = new Seat { Id = seatId, Sector = sector, SectorId = "Sec_1", SeatLabel = "A1" };
            _context.Sectors.Add(sector);
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            var request = new BookingCheckoutRequestDto
            {
                EventOccurrenceId = occId,
                CustomerEmail = "test@test.com",
                CustomerName = "Valid User",
                CustomerPhone = "+36301234567",
                SeatIds = new List<string> { seatId }
            };

            _mockQrService.Setup(x => x.GenerateTicketQrCode(It.IsAny<string>())).Returns("QR_TICKET_BASE64");
            _mockQrService.Setup(x => x.GenerateReservationQrCode(It.IsAny<string>())).Returns("QR_RES_BASE64");

            // Act
            var result = await _reservationService.CheckoutReservation(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(occId, result.EventId);
            Assert.AreEqual("USD", result.Currency);
            Assert.AreEqual(1000m, result.TotalPrice); // Extracted from Sector.BasePrice
            Assert.AreEqual("QR_RES_BASE64", result.QrCodeBase64);
            Assert.AreEqual(1, result.Tickets.Count);
            Assert.AreEqual("A1", result.Tickets[0].SeatLabel);
            Assert.AreEqual("QR_TICKET_BASE64", result.Tickets[0].QrCodeBase64);

            // Verify Email Service was called
            _mockEmailService.Verify(e => e.SendBookingConfirmationAsync(
                "test@test.com",
                "Valid User",
                "Test Event",
                It.IsAny<DateTime>(),
                It.Is<IEnumerable<EmailTicketItem>>(t => t.Count() == 1 && t.First().Price == 1000m),
                1000m,
                "USD"
            ), Times.Once);

            // Verify database state
            var savedRes = _context.Reservations.Include(r => r.ReservationSeats).FirstOrDefault(r => r.Id == result.BookingId);
            Assert.IsNotNull(savedRes);
            Assert.AreEqual("Confirmed", savedRes.Status);
            Assert.AreEqual(1, savedRes.ReservationSeats.Count);
        }
    }
}