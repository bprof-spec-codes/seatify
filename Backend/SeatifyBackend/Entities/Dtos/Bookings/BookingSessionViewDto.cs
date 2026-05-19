using System;
using System.Collections.Generic;

namespace Entities.Dtos.Bookings
{
    public class BookingSessionViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string EventOccurrenceId { get; set; } = string.Empty;
        public string Phase { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public List<SeatHoldViewDto> Holds { get; set; } = new();
    }

    public class SeatHoldViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string SeatId { get; set; } = string.Empty;
        public string RowLabel { get; set; } = string.Empty;
        public string SeatLabel { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
    }
}
