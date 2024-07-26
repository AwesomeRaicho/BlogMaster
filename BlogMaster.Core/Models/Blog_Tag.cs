using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Blog_Tag
    {
        public Guid BlogId { get; set; }
        public Blog? Blog { get; set; }
        public Guid TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
