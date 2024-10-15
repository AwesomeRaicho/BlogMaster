using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogMaster.Core.Models;

namespace BlogMaster.Core.DTO
{
    public class BlogPostPutDto
    {
        [MaxLength(255, ErrorMessage = "Max 255 characters for English Title ")]
        public string? TitleEn { get; set; }  // English title
        
        [MaxLength(255, ErrorMessage = "Max 255 characters for Spanish Title ")]
        public string? TitleEs { get; set; }  // Spanish title

        public string? DescriptionEn { get; set; }  // English description
        public string? DescriptionEs { get; set; }  // Spanish description
        public string? ArticleEn { get; set; }  // English article
        public string? ArticleEs { get; set; }  // Spanish article




        public string? IsFeatured { get; set; }
        public string? IsPublished { get; set; }
        public string? IsSubscriptionRequired { get; set; }

        [Required(ErrorMessage = "Blog author is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "min 6 charactes and max 50 characters for the Author")]
        public string? Author { get; set; }

        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }

        //categories, tags and keywords
        public List<string>? CategoriesIds { get; set; }
        public List<string>? TagsIds { get; set; }
        public List<string>? KeywordsIds { get; set; }

        //images in add/remove 
        public List<ImageViewDto>? ImageViews{ get; set; }
        
    }
}
