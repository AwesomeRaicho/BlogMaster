using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogAndRecomendations
    {
        public BlogResponseDto? Blog {  get; set; }

        public BlogPreviewsDto? BlogPreviews { get; set;}
    }
}
