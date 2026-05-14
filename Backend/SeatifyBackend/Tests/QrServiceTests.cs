using NUnit.Framework;
using Logic.Services;
using System;

namespace Tests
{
    [TestFixture]
    public class QrServiceTests
    {
        private QrService _qrService;

        [SetUp]
        public void SetUp()
        {
            _qrService = new QrService();
        }

        [Test]
        public void GenerateReservationQrCode_ValidId_ReturnsValidBase64String()
        {
            // Arrange
            string reservationId = "Res_12345";

            // Act
            string result = _qrService.GenerateReservationQrCode(reservationId);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(result), "The generated QR code should not be empty.");

            // If conversion does not throw an exception, it is a valid Base64 string
            Assert.DoesNotThrow(() => Convert.FromBase64String(result), "The returned value is not a valid Base64 string.");
        }

        [Test]
        public void GenerateTicketQrCode_ValidId_ReturnsValidBase64String()
        {
            // Arrange
            string seatId = "Seat_98765";

            // Act
            string result = _qrService.GenerateTicketQrCode(seatId);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(result), "The generated QR code should not be empty.");
            Assert.DoesNotThrow(() => Convert.FromBase64String(result), "The returned value is not a valid Base64 string.");
        }

        [Test]
        public void GenerateQrCodeBase64_DifferentInputs_ReturnDifferentStrings()
        {
            // Arrange
            string input1 = "Reservation:Res_1";
            string input2 = "Ticket:Seat_1";

            // Act
            string result1 = _qrService.GenerateQrCodeBase64(input1);
            string result2 = _qrService.GenerateQrCodeBase64(input2);

            // Assert
            Assert.AreNotEqual(result1, result2, "Different inputs should yield different Base64 results.");
        }

        [Test]
        public void GenerateQrCodeBase64_SameInputs_ReturnSameString()
        {
            // Arrange
            string input = "SomeText123";

            // Act
            string result1 = _qrService.GenerateQrCodeBase64(input);
            string result2 = _qrService.GenerateQrCodeBase64(input);

            // Assert
            Assert.AreEqual(result1, result2, "The same input should always return the same Base64 result (deterministic).");
        }
    }
}