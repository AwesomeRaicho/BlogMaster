using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class PasswordResetDto
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; } = null;

        public string? ConfirmNewPassword { get; set; }

        public string? Token { get; set; }
        public string? UserId { get; set; }




    }
}
