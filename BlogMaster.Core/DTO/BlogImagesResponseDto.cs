using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogImagesResponseDto
    {
        public Guid BlogImageId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? Url { get; set; }
        public string? ImageName { get; set; }
        public string? MimeType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
