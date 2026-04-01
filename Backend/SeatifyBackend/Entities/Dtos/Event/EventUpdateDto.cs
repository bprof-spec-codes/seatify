using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Event
{
    public class EventUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Slug { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}