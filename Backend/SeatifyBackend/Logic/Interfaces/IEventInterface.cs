using Entities.Dtos.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IEventService
    {
        public bool CreateEvent(EventCreateDto dto);
        public EventViewDto? GetById(string id);
        public List<EventViewDto> GetAll();
        public bool UpdateEvent(string id, EventUpdateDto dto);
        public bool DeleteEvent(string id);
    }
}