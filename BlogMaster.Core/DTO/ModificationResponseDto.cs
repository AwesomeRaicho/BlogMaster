using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class ModificationResponseDto
    {
        public Guid ModificationId { get; set; }
        public string? Description { get; set; }
        public DateTime ModificationDate { get; set; }

        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }

    }
}
