using Logic.Interfaces;
using Logic.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class PdfServiceTests
    {
        private PdfService _pdfService;

        [SetUp]
        public void Setup()
        {
            _pdfService = new PdfService();
        }

        [Test]
        public void GenerateTicketPdf_ValidInputs_ReturnsByteArray()
        {
            // Arrange
            var tickets = new List<PdfTicketItem>
            {
                new PdfTicketItem
                {
                    SeatLabel = "A1",
                    Row = "A",
                    SeatNumber = "1",
                    Price = 5000,
                    QrCodeBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=", // 1x1 transparent png in base64
                    ManualCode = "TEST-123"
                }
            };

            // Act
            var pdfBytes = _pdfService.GenerateTicketPdf(
                "Test Event",
                "Test Venue",
                "Test Auditorium",
                DateTime.UtcNow,
                tickets,
                "USD",
                "#ff0000"
            );

            // Assert
            Assert.IsNotNull(pdfBytes);
            Assert.IsTrue(pdfBytes.Length > 0);
            
            // PDF files start with %PDF-
            var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 5);
            Assert.AreEqual("%PDF-", pdfHeader);
        }
    }
}
