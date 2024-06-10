using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class TimeSlotModel
    {
        [Required]
        public TimeOnly SlotStartTime { get; set; }

        [Required]
        public TimeOnly SlotEndTime { get; set; }
    }
}
