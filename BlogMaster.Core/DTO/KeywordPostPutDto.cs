using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class KeywordPostPutDto
    {
        public Guid KeywordId { get; set; }
        public string? KeywordNameEn { get; set; }
        public string? KeywordNameEs { get; set; }
    }
}
