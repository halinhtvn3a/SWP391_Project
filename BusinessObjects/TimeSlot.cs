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
		[StringLength(5)]
		public string SlotId { get; set; }

		[ForeignKey("Court")]
		[StringLength(5)]
		public string CourtId { get; set; }

		[Required]
		public DateTime SlotDate { get; set; }

		[Required]
		public TimeSpan SlotStartTime { get; set; }

		[Required]
		public TimeSpan SlotEndTime { get; set; }

		public bool? IsAvailable { get; set; }

		// Navigation property
		public Court Court { get; set; }
		public ICollection<Booking> Bookings { get; set; }
	}
}
