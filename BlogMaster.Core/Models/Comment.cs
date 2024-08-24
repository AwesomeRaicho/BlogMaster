using BlogMaster.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public Guid BlogId { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }

        public Guid UserId { get; set; }
        public DateTime Created { get; set; }

        //Navigation prop
        public ApplicationUser? User { get; set; }
        public Blog? Blog { get; set; }
    }
}
