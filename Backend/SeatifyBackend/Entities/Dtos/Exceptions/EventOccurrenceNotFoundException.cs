using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Exceptions
{
    public class EventOccurrenceNotFoundException: Exception
    {
        public EventOccurrenceNotFoundException(string msg): base(msg)
        {
                
        }
    }
}
