using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
	public class Branch
	{
		[Key]
		[StringLength(5)]
		public string BranchId { get; set; }

		[Required]
		[StringLength(255)]
		public string Address { get; set; }

		[StringLength(255)]
		public string Description { get; set; }

		[Required]
		[StringLength(255)]
		public string Picture { get; set; }

		[Required]
		public TimeSpan OpenTime { get; set; }

		[Required]
		public TimeSpan CloseTime { get; set; }

		[Required]
		[StringLength(255)]
		public string OpenDay { get; set; }

		[Required]
		public bool Status { get; set; }

		// Navigation property
		public ICollection<Court> Courts { get; set; }
	}
}
