using Entities.Dtos.EventOccurrence;
using Entities.Dtos.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IEventOccurrenceService
    {
        public bool Create(EventOccurrenceCreateDto createDto);
        public EventOccurrenceViewDto? GetById(string id);
        public bool Update(string id, EventOccurrenceCreateDto updateDto);
        public bool Delete(string id);
        public List<ReservationViewDto> GetReservations(string id);
    }
}
