using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Exceptions
{
    public class EventNotFoundException: Exception
    {
        public EventNotFoundException(string msg): base(msg)
        {
            
        }
    }
}
