using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BusinessObjects
{
	public class UserDetail
	{
		[Key]
		[StringLength(5)]
		public string UserDetailsId { get; set; }

		public decimal? Balance { get; set; }

		[StringLength(50)]
		public string FullName { get; set; }

		[Required]
		public bool Status { get; set; }

		// Navigation property
		public IdentityUser User { get; set; }
	}
}
