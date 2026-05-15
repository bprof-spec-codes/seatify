using Entities.Dtos.Bookings;

namespace Logic.Interfaces
{
    public interface IBookingSessionService
    {
        BookingSessionViewDto Create(BookingSessionCreateDto dto);
        BookingSessionViewDto? GetById(string bookingSessionId);
        BookingSessionViewDto? HoldSeat(string bookingSessionId, BookingSessionHoldDto dto);
        BookingSessionViewDto? ReleaseSeat(string bookingSessionId, string seatId);
        BookingSessionViewDto? MoveToCheckout(string bookingSessionId);
    }
}
