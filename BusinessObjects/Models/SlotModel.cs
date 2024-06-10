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
        public string? CourtId { get; set; } 
        
        [StringLength(10)]
        public string? BranchId { get; set; }

        public DateOnly SlotDate { get; set; }

        [Required]
        public TimeSlotModel TimeSlot { get; set; }
    }
}
