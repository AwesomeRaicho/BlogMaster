using BlogMaster.Core.Models.Identity;
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
        public decimal? RatingScore { get; set; }
        
        public Guid BlogId { get; set; }
        public Guid UserId { get; set;}

        //Navigation 
        public ApplicationUser? User { get; set;}
        public Blog? Blog { get; set; }
    }
}
