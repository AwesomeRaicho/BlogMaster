using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Rating
    {
        public Guid RatingId { get; set; }
        public string? UserName { get; set;}
        public decimal? RatingScore { get; set; }

        //Navigation 
        public Blog? Blog { get; set; }
    }
}
