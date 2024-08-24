using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Blog_Keyword
    {
        public Guid BlogId { get; set; }
        public Blog? Blog { get; set; }
        public Guid KeywordId { get; set; }
        public Keyword? Keyword { get; set; }
    }
}
