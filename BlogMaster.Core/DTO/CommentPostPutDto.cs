using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class CommentPostPutDto
    {
        public Guid CommmentId { get; set; }
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }

        public string? Message { get; set; }

    }
}
