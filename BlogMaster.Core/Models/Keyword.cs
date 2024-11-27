using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Keyword
    {
        public Guid KeywordId { get; set; }
        public string? KeywordNameEn { get; set; }

        // Navigation
        public List<Blog_Keyword>? BlogKeywords { get; set; }
    }
}
