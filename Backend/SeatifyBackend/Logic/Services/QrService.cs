using Logic.Interfaces;
using QRCoder;

namespace Logic.Services
{
    public class QrService : IQrService
    {
        const string QR_CODE_PREFIX = "Reservation:";
        const string TICKET_QR_CODE_PREFIX = "Ticket:";

        public virtual string GenerateReservationQrCode(string reservationId)
        {
            return GenerateQrCodeBase64(QR_CODE_PREFIX + reservationId);
        }

        public virtual string GenerateTicketQrCode(string reservationSeatId)
        {
            return GenerateQrCodeBase64(TICKET_QR_CODE_PREFIX + reservationSeatId);
        }

        public string GenerateQrCodeBase64(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var pngQr = new PngByteQRCode(qrData);
            byte[] qrBytes = pngQr.GetGraphic(5);

            // Convert byte array to Base64 string
            return Convert.ToBase64String(qrBytes);
        }
    }
}

