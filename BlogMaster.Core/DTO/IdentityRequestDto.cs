using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class IdentityRequestDto
    {
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? EmailAddress { get; set; }
        
        public string? Password { get; set; } = null;
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }


    }
}
