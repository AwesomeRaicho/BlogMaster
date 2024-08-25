using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class GetFormRequestDto
    {
        
        public int amount {  get; set; }
        public string? currency { get; set; }
        public string? productName { get; set; }
    }
}
