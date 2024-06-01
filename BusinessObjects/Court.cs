using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
	public class Court
	{
		[Key]
		[StringLength(5)]
		public string CourtId { get; set; }

		[ForeignKey("Branch")]
		[StringLength(5)]
		public string BranchId { get; set; }

		[StringLength(100)]
		public string CourtName { get; set; }

		[Required]
		public bool Status { get; set; }

		// Navigation properties
		public Branch Branch { get; set; }
		public ICollection<TimeSlot> TimeSlots { get; set; }
		public ICollection<Review> Reviews { get; set; }
	}
}
