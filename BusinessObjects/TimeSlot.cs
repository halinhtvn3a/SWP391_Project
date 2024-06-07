using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
	public class TimeSlot
	{
		[Key]
		[StringLength(10)]
		public string SlotId { get; set; }

		[ForeignKey("Court")]
		[StringLength(10)]
		public string CourtId { get; set; }
        
        [ForeignKey("Booking")]
		[StringLength(10)]
		public string? BookingId { get; set; } = null;

		[Required]
		public DateOnly SlotDate { get; set; }
        
        [Required]
        public decimal Price { get; set; }

        [Required]
		public TimeOnly SlotStartTime { get; set; }

		[Required]
		public TimeOnly SlotEndTime { get; set; }

        [Required]
        public bool? IsAvailable { get; set; }

		// Navigation property
		public virtual Court Court { get; set; }
        public virtual Booking? Booking { get; set; }
    }
}
