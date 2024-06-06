using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class CourtModel
    {
        [Required(ErrorMessage = "Branch ID is required")]
        [StringLength(10)]
        public string BranchId { get; set; }

        [Required(ErrorMessage = "Court name is required")]
        [StringLength(100)]
        public string CourtName { get; set; }

        [Required(ErrorMessage = "Court picture is required")]
        [StringLength(450)]
        public string CourtPicture { get; set; }

        [Required(ErrorMessage = "Court Status is required")]
        [StringLength(100)]
        public string Status { get; set; }
    }
}
