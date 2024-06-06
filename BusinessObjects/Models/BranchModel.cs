using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class BranchModel
    {
        [Required(ErrorMessage = "Branch address is required")]
        [StringLength(255)]
		public string BranchAddress { get; set; }

        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(255)]
		public string BranchName { get; set; }
        
        [Required(ErrorMessage = "Branch phone is required")]
		[StringLength(15)]
		public string BranchPhone { get; set; }

		[StringLength(255)]
		public string Description { get; set; }

		[StringLength(255)]
		public string BranchPicture { get; set; }

		[Required(ErrorMessage = "Open time is required")]
		public TimeOnly OpenTime { get; set; }

		[Required(ErrorMessage = "Close time is required")]
		public TimeOnly CloseTime { get; set; }

		[Required(ErrorMessage = "Open day is required")]
		[StringLength(255)]
		public string OpenDay { get; set; }

		[Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        public string Status { get; set; }
    }
}
