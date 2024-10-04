using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogEditViewDto
    {
        public BlogResponseDto? BlogResponseDto { get; set; }

        public List<CategoryResponseDto>? AllCategories{ get; set;}
        public List<TagResponseDto>? AllTags { get; set; }
        public List<KeywordResponseDto>? AllKeywords { get; set; }

    }
}
