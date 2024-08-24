using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class CommentResponseDto
    {
        public Guid CommentId { get; set; }
        public Guid BlogId { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public DateTime Created { get; set; }

        public Guid UserId { get; set; }
    }
}
