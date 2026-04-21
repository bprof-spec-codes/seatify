namespace Entities.Dtos.EventOccurrence
{
    public class EventOccurrenceViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string VenueId { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;

        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public DateTime BookingOpenAtUtc { get; set; }
        public DateTime BookingCloseAtUtc { get; set; }
        public DateTime? DoorsOpenAtUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public EventOccurrenceEventDto Event { get; set; } = null!;
        public EventOccurrenceVenueDto Venue { get; set; } = null!;
        public EventOccurrenceAuditoriumDto Auditorium { get; set; } = null!;
    }

    public class EventOccurrenceEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class EventOccurrenceVenueDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class EventOccurrenceAuditoriumDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
