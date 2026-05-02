using QRCoder;

namespace Logic.Services;

public class QrService
{
    const string QR_CODE_PREFIX = "Reservation:";
    const string TICKET_QR_CODE_PREFIX = "Ticket:";

    public string GenerateReservationQrCode(string reservationId)
    {
        return GenerateQrCodeBase64(QR_CODE_PREFIX + reservationId);
    }

    public string GenerateTicketQrCode(string reservationSeatId)
    {
        return GenerateQrCodeBase64(TICKET_QR_CODE_PREFIX + reservationSeatId);
    }

    public string GenerateQrCodeBase64(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(qrData);
        byte[] qrBytes = pngQr.GetGraphic(20);

        // A byte tömb átalakítása Base64 stringgé
        return Convert.ToBase64String(qrBytes);
    }
}
