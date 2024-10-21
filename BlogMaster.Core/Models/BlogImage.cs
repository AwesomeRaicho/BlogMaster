using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class BlogImage
    {
        public Guid ImageId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? Url { get; set; }
        public string? ImageName { get; set; }
        public string? MimeType { get; set; }
        public Guid BlogId { get; set; }
        public DateTime CreatedDate { get; set; }


        //Navigation Prop

        public Blog? Blog { get; set; }
    }
}
