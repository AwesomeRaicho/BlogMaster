using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class PublicBlogListDto
    {
        public Guid BlogId { get; set; }
        public string? TitleEn { get; set; }  // English title
        public string? TitleEs { get; set; }  // Spanish title
        public string? DescriptionEn { get; set; }  // English description
        public string? DescriptionEs { get; set; }  // Spanish description
        public string? SlugEn { get; set; }  // English slug for links
        public string? SlugEs { get; set; }  // Spanish slug for links
        public string? Author { get; set; }
        public DateTime? DatePublished { get; set; }
        public decimal? AverageRating { get; set; }

    }
}
