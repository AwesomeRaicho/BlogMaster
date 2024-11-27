using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Tag
    {
        public Guid TagId { get; set; }
        public string? TagNameEn {  get; set; }


        // Navigation prop
        public List<Blog_Tag>? BlogTags { get; set; }
    }
}
