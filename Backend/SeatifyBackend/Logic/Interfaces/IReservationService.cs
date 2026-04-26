using Entities.Dtos.Bookings;
using Entities.Dtos.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IReservationService
    {
        public bool CreateReservation(string eventOccurrenceId, ReservationCreateDto dto);
        public ReservationViewDto? GetById(string id);
        public List<ReservationViewDto> GetByOccurrenceId(string eventOccurrenceId);
        public bool UpdateReservation(string id, ReservationUpdateDto dto);
        public bool DeleteReservation(string id);
        public Task<BookingCheckoutResponseDto> CheckoutReservation(BookingCheckoutRequestDto request);
    }
}
