namespace Logic.Interfaces
{
    public interface IQrService
    {
        string GenerateReservationQrCode(string reservationId);
        string GenerateTicketQrCode(string reservationSeatId);
        string GenerateQrCodeBase64(string text);
    }
}
