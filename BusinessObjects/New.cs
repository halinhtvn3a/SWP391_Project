using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class News
    {
        [Key]
        [StringLength(10)]
        public string NewId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Content { get; set; }

        [Required]
        public DateTime PublicationDate { get; set; }

        [StringLength(255)]
        public string Image { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        public bool IsHomepageSlideshow { get; set; }
    }
}

