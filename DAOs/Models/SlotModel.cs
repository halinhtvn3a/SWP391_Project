using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class SlotModel
    {
        [StringLength(10)]
        [Required]
        public string CourtId { get; set; }

        [Required]
        public DateOnly SlotDate { get; set; }

        [Required]
        public TimeOnly SlotStartTime { get; set; }

        [Required]
        public TimeOnly SlotEndTime { get; set; }
    }
}
