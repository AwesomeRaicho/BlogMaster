using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set;}


        //Navigation
        //public Subscription? Subscription { get; set; }
        
            
        public List<Blog>? Blogs { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Modification>? Modifications { get; set; }
        public List<Rating>? Ratings { get; set; }
    }
}
 