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
        public Guid UsedId { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }


        //Navigation prop
        public Blog? Blog { get; set; }
    }
}
