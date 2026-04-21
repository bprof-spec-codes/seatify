using System;

namespace Entities.Dtos.CheckIn
{
    public class CheckInValidateRequest
    {
        public string Payload { get; set; } = string.Empty;
    }

    public class CheckInConfirmRequest
    {
        public string TicketId { get; set; } = string.Empty;
    }

    public class CheckInResult
    {
        public string TicketId { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

        public ReservationInfo? Reservation { get; set; }
        public SeatInfo? Seat { get; set; }
        public EventInfo? Event { get; set; }
    }

    public enum TicketStatus
    {
        Valid,
        AlreadyUsed,
        Invalid
    }

    public class ReservationInfo
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class SeatInfo
    {
        public string Section { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Number { get; set; }
    }

    public class EventInfo
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
    }
}
