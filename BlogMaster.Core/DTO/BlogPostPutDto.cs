using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMaster.Core.DTO
{
    public class BlogPostPutDto
    {
        [Required(ErrorMessage = "Blog Article is required")]
        [StringLength(3600, MinimumLength = 300, ErrorMessage = "Max 3600 characters in Article")]
        public string? Article { get; set; }

        [Required(ErrorMessage = "Blog title is required")]
        [MaxLength(50)]
        public string? Title { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Blog category is required")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Max 30 characters in Category")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Blog author is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "min 6 charactes and max 50 characters for the Author")]
        public string? Author { get; set; }
    }
}
