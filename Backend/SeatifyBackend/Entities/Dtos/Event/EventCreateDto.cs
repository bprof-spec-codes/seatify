using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Event
{
    public class EventCreateDto
    {
        public string OrganizerId { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Currency { get; set; }
        public string Status { get; set; } = "Published";
    }
}