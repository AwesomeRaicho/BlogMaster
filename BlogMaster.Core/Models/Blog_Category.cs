using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Blog_Category
    {
        public Guid BlogId { get; set; }
        public Guid CategoryId { get; set; }
        
        
        // Navigation props
        public Blog? Blog { get; set; }
        public Category? Category { get; set; }
    }
}
