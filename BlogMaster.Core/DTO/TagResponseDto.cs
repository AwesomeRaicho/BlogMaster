using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class TagResponseDto
    {
        public Guid TagId { get; set; }
        public string? TagNameEn { get; set; }
        public string? TagNameEs { get; set; }
    }
}
