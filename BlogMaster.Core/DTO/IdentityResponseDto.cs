using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class IdentityResponseDto
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }

    }
}
