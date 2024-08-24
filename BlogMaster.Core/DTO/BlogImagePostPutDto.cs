using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogImagePostPutDto
    {
        public Guid BlogId { get; set; }
        public Guid BlogImageId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? Url { get; set; }
        [Required]
        public string? ImageName { get; set; }
        [Required] 
        public string? MimeType { get; set; }
    }
}
