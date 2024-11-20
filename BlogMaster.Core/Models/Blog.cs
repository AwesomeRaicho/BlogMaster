using BlogMaster.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Blog
    {
        public Guid BlogId { get; set; }

        //<IMAGES PROPs HERE>

        // Language-specific fields
        public string? ArticleEn { get; set; }  // English article
        public string? TitleEn { get; set; }  // English title
        public string? DescriptionEn { get; set; }  // English description
        public string? SlugEn { get; set; }  // English slug for links

        public string? Author { get; set; }

        public bool? IsFeatured { get; set; }

        public bool? IsPublished { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DatePublished { get; set; }



        public int? ViewCount { get; set; }
        public decimal? AverageRating { get; set; }
        public int? RatingCount { get; set; }
        public bool IsSubscriptionRequired { get; set; }

        public Guid UserId { get; set; }

        //Navigation Properties
        public ApplicationUser? User { get; set; }
        
        //  Many-to-Many
        public List<Blog_Keyword>? BlogKeywords { get; set; }
        public List<Blog_Tag>? BlogTags { get; set; }
        public List<Blog_Category>? BlogCategories { get; set; }

        //  One-to-Many
        public List<BlogImage>? BlogImages { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Rating>? Ratings { get; set; }
        public List<Modification>? Modifications { get; set; }
    }
}
