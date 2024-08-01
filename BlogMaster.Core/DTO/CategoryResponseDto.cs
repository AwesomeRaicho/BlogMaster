using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class CategoryResponseDto
    {
        public Guid CategoryId { get; set; }

        public string? CatergoryNameEn { get; set; }
        public string? CatergoryNameEs { get; set; }
    }
}
