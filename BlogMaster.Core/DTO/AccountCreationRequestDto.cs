using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class AccountCreationRequestDto
    {
        [Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? RoleName { get; set; }
    }
}
