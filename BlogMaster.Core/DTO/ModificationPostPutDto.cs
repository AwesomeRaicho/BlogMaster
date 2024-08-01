using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class ModificationPostPutDto
    {
        public Guid BlogId { get; set; }
        public string? Description { get; set; }

    }
}
