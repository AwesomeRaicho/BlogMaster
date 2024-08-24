using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class SignInResponseDto
    {
        public bool IsSeccess { get; set; }
        public string? ErrorMessage {  get; set; }
    }
}
