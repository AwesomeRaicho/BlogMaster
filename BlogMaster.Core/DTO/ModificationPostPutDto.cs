using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class ModificationPostPutDto
    {
        public Guid UserId { get; set; }
        public Guid BlogId { get; set; }
        [Required]
        public string? Description { get; set; }

    }
}
