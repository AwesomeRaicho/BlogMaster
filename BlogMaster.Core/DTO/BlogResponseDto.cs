using BlogMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogResponseDto
    {
        public Guid BlogId { get; set; }
        public string? ArticleEn { get; set; }  // English article
        public string? ArticleEs { get; set; }  // Spanish article
        public string? TitleEn { get; set; }  // English title
        public string? TitleEs { get; set; }  // Spanish title
        public string? DescriptionEn { get; set; }  // English description
        public string? DescriptionEs { get; set; }  // Spanish description
        public string? SlugEn { get; set; }  // English slug for links
        public string? SlugEs { get; set; }  // Spanish slug for links
        public string? Author { get; set; }
        public DateTime? DatePublished { get; set; }
        public int? ViewCount { get; set; }
        public decimal? AverageRating { get; set; }
        public int? RatingCount { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsSubscriptionRequired { get; set; }

        public List<CommentRatingResponseDto>? CommentsRatings { get; set; } 
        public List<CategoryResponseDto>? Categories { get; set; }
        public List<KeywordResponseDto>? Keywords { get; set; } 
        public List<BlogImagesResponseDto>? BlogImages { get; set; } 
        public List <TagResponseDto>? Tags { get; set; }

    }
}
