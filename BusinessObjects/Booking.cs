using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BusinessObjects
{
	public class Booking
	{
		[Key]
		[StringLength(5)]
		public string BookingId { get; set; }

		[ForeignKey("User")]
		[StringLength(450)]
		public string Id { get; set; }

		[ForeignKey("TimeSlot")]
		[StringLength(5)]
		public string SlotId { get; set; }

		[Required]
		public DateTime BookingDate { get; set; }

		[Required]
		public bool Check { get; set; }

		[Required]
		public decimal PaymentAmount { get; set; }

		// Navigation properties
		public IdentityUser User { get; set; }
		public TimeSlot TimeSlot { get; set; }
		public ICollection<Payment> Payments { get; set; }
	}
}
