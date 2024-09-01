using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class GetFormRequestDto
    {
        
        public int Amount {  get; set; }
        public string? Currency { get; set; }
        public string? Mode { get; set; }
        public string? Username { get; set; }
        public string? UserEmail { get; set; }
    }
}
