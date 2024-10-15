using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class ImageViewDto
    {
        public string? src { get; set; }
        public string? Filename { get; set; }
        public string? MimeType { get; set; }
        public Guid ImageId { get; set; }
    }
}
