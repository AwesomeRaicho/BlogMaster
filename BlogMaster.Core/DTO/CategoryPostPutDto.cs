using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class CategoryPostPutDto
    {
        public Guid CategoryId { get; set; }


        public string? CategoryNameEn { get; set; }
        public string? CategoryNameEs { get; set; }
    }
}
