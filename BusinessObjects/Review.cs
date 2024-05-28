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
	public class Review
	{
		[Key]
		[StringLength(5)]
		public string ReviewId { get; set; }

		[StringLength(255)]
		public string ReviewText { get; set; }

		public DateTime? ReviewDate { get; set; }

		public int? Rating { get; set; }

		[ForeignKey("User")]
		[StringLength(450)]
		public string Id { get; set; }

		[ForeignKey("Court")]
		[StringLength(5)]
		public string CourtId { get; set; }

		// Navigation properties
		public IdentityUser User { get; set; }
		public Court Court { get; set; }
	}
}
